function onDelete(){
    $('#confirmDelete').show();
    $('#delete').hide();
}

function dropDelete(){
    $('#confirmDelete').hide();
    $('#delete').show();
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

function add(){
    rateInput.value = parseFloat(rateInput.value) + rateStep;
}

function minus(){
    let newValue = parseFloat(rateInput.value) - rateStep;
    console.log(newValue)
    if (newValue > minRate) {
        rateInput.value = parseFloat(newValue);
    }
}



async function onBackClick(){
    fundedSpan.innerHTML = rateInput.value;
    rateCount.innerHTML = parseFloat(rateCount.innerHTML) + 1;
    
    let rate = rateInput.value
    
    let response = await fetch(window.location.origin + `/rate/SetRate?lotId=${lotId}&rate=${rate}`, {
        method: 'post',
        headers: getFormHeader()
    })
    
    console.log(rate)
}

async function updateLotData(){
    let response = await fetch(window.location.origin + `/rate/GetLotFunding?lotId=${lotId}`, {
        method: 'get',
        headers: getFormHeader()
    })

    if (response.ok){
        let data = await response.json();
        fundedSpan.innerHTML = data.currentPrice;
        rateCount.innerHTML = data.ratesCount;
        minRate = data.currentPrice;
        
        return setTimeout(updateLotData, 2000);
    }
}

updateLotData()


async function onLaunchClick(){
    let response = await fetch(window.location.origin + `/lot/LaunchProject?lotId=${lotId}`,{
        method: 'post',
        headers: getFormHeader()
    });
    
    console.log(response);
    if(response.ok){
        console.log(await response.json())
    }
}