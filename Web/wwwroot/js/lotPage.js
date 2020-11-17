"use strict"


function onDelete(){
    document.getElementById('confirmDelete').style.display = '';
    document.getElementById('delete').style.display = 'none';
}

function dropDelete(){
    document.getElementById('confirmDelete').style.display = 'none';
    document.getElementById('delete').style.display = '';
}

document.getElementById('add-bet')?.addEventListener('click', add);
document.getElementById('minus-bet')?.addEventListener('click', minus);
document.getElementById('launch-bth')?.addEventListener('click', launch)

async function launch(e){
    e.preventDefault();
    
    let urlProps = '/Lot/LaunchProject?lotId=' + lotId; 
    let response = await fetch(window.location.origin + urlProps, {
        method: 'post',
        headers: getFormHeader()
    });
    
    if(response.ok){
        location.reload();
    }
    else{
        alert('fill all fields in the project');
    }
}

let goal = document.getElementById('min-price').innerText;
goal = goal.replace(',', '.');
document.getElementById('min-price').innerText = goal;

let rateInput = document.getElementById('rate-input');
let fundedSpan = document.getElementById('funded-span');
fundedSpan.innerText = fundedSpan.innerText.replace(',', '.');
rateInput.value = fundedSpan.innerText

let lotId = document.getElementById('Id').value;
let rateCount = document.getElementById('rate-count');

function getMinRate() {
    let rate = parseFloat(rateInput.value);
    let price = parseFloat(fundedSpan.innerText);
    
    return rate > price? rate : price;
}

let minRate = getMinRate();
const rateStep = 10.0;

function add() {
    rateInput.value = parseFloat(rateInput.value) + rateStep;
}

function minus() {
    let newValue = parseFloat(rateInput.value) - rateStep;
    if (newValue > minRate) {
        rateInput.value = parseFloat(newValue);
    }
}

rateInput.addEventListener('input', (e) => {
    rateInput.value = rateInput.value.replace(/[^0-9.-]/g, '')

    let regexp = /^\$?[0-9]*[0-9]\.?[0-9]{0,2}$/i;
    if(!regexp.test(rateInput.value)){
        let temp = rateInput.value.split('.');
        temp[1] = temp[1].substr(0, 2);
        rateInput.value = temp.join('.');
    }
});



let backButton = document.getElementById("back-button");
if (backButton){
    backButton.disabled = true;
}




const connection = new signalR.HubConnectionBuilder()
    .withUrl('/betHub')
    .build();

connection.on('UpdateBet', (bet) => {
    fundedSpan.innerHTML = bet.amount;
    rateCount.innerHTML = +rateCount.innerHTML + 1;
    minRate = bet.amount;
})

connection.on('Exception', (message) => {
    console.log(message);
})

connection.start().then(function () {
    if (backButton){
        backButton.disabled = false;
    }
    
    connection.invoke("JoinRoom", lotId)
        .catch(function (err) {
            return console.error(err.toString());
        });

    backButton?.addEventListener('click', onBackClick)
    
}).catch(function (err) {
    return console.error(err.toString());
});

function onBackClick(){
    let bet = rateInput.value;
    if (bet > minRate){
        connection.invoke('AddBet', lotId, bet)
            .catch(function (err) {
                return console.error(err.toString());
            });
    }
}


window.onbeforeunload = () => {
    connection.invoke('LeaveRoom', lotId);
}