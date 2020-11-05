"use strict"


function onDelete(){
    $('#confirmDelete').show();
    $('#delete').hide();
}

function dropDelete(){
    $('#confirmDelete').hide();
    $('#delete').show();
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


function parseNumber(value, locale = navigator.language) {
    const example = Intl.NumberFormat(locale).format('1.1');
    const cleanPattern = new RegExp(`[^-+0-9${ example.charAt( 1 ) }]`, 'g');
    const cleaned = value.replace(cleanPattern, '');
    const normalized = cleaned.replace(example.charAt(1), '.');

    return parseFloat(normalized);
}


let rateInput = document.getElementById('rate-input');
let lotId = document.getElementById('Id').value;
let fundedSpan = document.getElementById('funded-span');
let rateCount = document.getElementById('rate-count');

// $("#rate-input").on("keypress keyup blur",function (event) {
//     $(this).val($(this).val().replace(/[^0-9\.]/g,''));
//            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
//                event.preventDefault();
//            }
//        });

let minRate = parseFloat(rateInput.value);
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


let backButton = document.getElementById("back-button");
if (backButton){
    backButton.disabled = true;
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl('/commentsHub')
    .build();

connection.on('UpdateBet', (bet) => {
    console.log(bet);
    
    fundedSpan.innerHTML = bet.amount;
    rateCount.innerHTML = parseInt(rateCount.innerHTML) + 1;
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
    let goal = document.getElementById('goal').innerText;
    if (bet < goal){
        return 
    }

    let culture = getCulture();
    connection.invoke('AddBet', lotId, bet, culture)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

rateInput.addEventListener('input', (i) => {
    let value = i.target.value;
    console.log(value);
    console.log('num', parseNumber(value))
    console.log(Intl.NumberFormat(local).format())
    let regexp = /^\d+$/;
    console.log(regexp.test(value))
    
    // console.log('event: ', parseFloat(i.target.value))
});



function getCulture(){
    let language;
    if (window.navigator.languages) {
        language = window.navigator.languages[0];
    } else {
        language = window.navigator.userLanguage || window.navigator.language;
    }
    
    return language;
}