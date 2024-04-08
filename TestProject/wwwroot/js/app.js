class Shop {
    constructor(checkout) {
        this.load()
            .then(() => this.initHandlers());
        
        this.products = [];
        this.checkout = checkout;
    }
    
    async load(){
        const resp = await fetch('/product');
        const res = await resp.json();
        this.products = res;
        
        let html = '';
        for(let p of res){
            html += `<li class="list-group-item">
                    <button class="px-0 btn w-100 text-start" data-id="${p.id}"> 
                        <span>${p.name}</span>
                        <span class="fw-bold float-end">${p.price}$</span>
                    </button>
                </li>`;
        }
        
        document.getElementById('shop-list').innerHTML = html;
    }
    
    initHandlers(){
        document.querySelectorAll('#shop-list .list-group-item .btn')
            .forEach(x => x.addEventListener('click', e => this.onProductClick(e)));
    }
    
    onProductClick(e){
        const id = +e.currentTarget.dataset.id;
        const product = this.products.find(x => x.id === id);
        if(product == null) return;
        
        this.checkout.addItem(id, product.name, product.price);
    }
}

class Checkout {
    constructor() {
        this.list = [];
        this.auth = null;
        
        document.getElementById('make-order')
            .addEventListener('click', () => this.makeOrder());
    }
        
    addItem(id, name, price){
        const item = this.list.find(x => x.id === id);
        if(!item){
            this.list.push({ id, name, price, quantity: 1 });
        }else{
            item.quantity += 1;
        }
        
        this.render();
    }
    
    removeItem(id){
        const idx = this.list.findIndex(x => x.id === id);
        if(!~idx) return;
        
        this.list.splice(idx, 1);
        
        this.render();
    }
    
    changeQuantity(id, quantity){
        const item = this.list.find(x => x.id === id);
        if(!item) return;
        
        item.quantity = quantity;
        this.render();
    }
    
    updateTotal(){
        const total = this.list.reduce((prevValue, x) => prevValue + x.price * x.quantity, 0);
        document.getElementById('total-label').innerText = total;
    }
    
    render(){
        let html = '';
        for(let p of this.list){
            html += `<li class="list-group-item text-end">
                    <span class="d-inline-block float-start text-truncate mt-2">${p.name}</span>
                    <div class="d-inline-block w-25 mx-1">
                        <input class="form-control text-end" type="number" value="${p.quantity}" data-id="${p.id}"/>
                    </div>
                    <span class="d-inline-block fw-bold">${p.price * p.quantity}$</span>
                    <button class="btn p-0" data-id="${p.id}">✖️</button>
                </li>`;
        }
        
        document.getElementById('checkout-list').innerHTML = html;
        
        document.querySelectorAll('#checkout-list .list-group-item .btn')
            .forEach(x => x.addEventListener('click', e => {
                this.removeItem(+e.target.dataset.id);
            }));
        document.querySelectorAll('#checkout-list .list-group-item input')
            .forEach(x => x.addEventListener('change', e => {
                this.changeQuantity(+e.target.dataset.id, +e.target.value);
            }));

        this.updateTotal();
    }
    
    async makeOrder() {
        const model = this.list.map(x => ({ productId: x.id, quantity: x.quantity }));
        
        const resp = await fetch('/order', {
            body: JSON.stringify(model),
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
                'Authorize': this.auth
            }
        });
        
        if(!resp.ok){
            alert('Error! ' + resp.statusText);
            return;
        }
        
        this.list = [];
    }


    show(){
        document.getElementById('checkout-list-wrap').classList.remove('d-none');
    }
    hide(){
        document.getElementById('checkout-list-wrap').classList.add('d-none');
    }
}

class Login {
    constructor(checkout) {
        this.isLoggedIn = false;
        this.checkout = checkout;
        
        document.getElementById('login')
            .addEventListener('click', () => this.login());
    }
    
    async login(){
        const email = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;
        
        const model = { email, password };
        const resp = await fetch('/users', {
            method: 'post',
            body: JSON.stringify(model),
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        });
        
        const res = await resp.json();
        
        this.setUser(res);
    }
    
    setUser(user){
        document.getElementById('user-name').innerText = user.name;
        document.getElementById('user-email').innerText = user.email;
        document.getElementById('user-phone').innerText = user.phone;
        
        this.checkout.auth = user.id;
        
        this.hide();
        this.checkout.show();
    }
    
    show(){
        document.getElementById('login-wrap').classList.remove('d-none');
    }
    hide(){
        document.getElementById('login-wrap').classList.add('d-none');
    }
}

;(function(){
    const checkout = new Checkout();
    const shop = new Shop(checkout);
    const login = new Login(checkout);
    
    checkout.hide();
    login.show();
})();