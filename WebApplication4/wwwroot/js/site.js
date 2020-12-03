"use strict"

document.cookie = "timezoneOffset=" + (- new Date().getTimezoneOffset());

function changeUrl(newUrl){
    if (history.pushState) {
        window.history.pushState('', '', newUrl);
    } 
    else {
        document.location.href = newUrl;
    }
}


function getFormHeader(){
    return {
        'X-ANTI-FORGERY-TOKEN': document.getElementsByName("__RequestVerificationToken")[0].value,
        'Accept': 'application/json',
        "Content-Type": "application/json",
    }
}


async function loadByMark(url, insertInto){
    let mark = document.getElementById('load-mark');
    if (mark !== null && mark.style['display'] !== 'none'){
        mark.remove();
        await load(url, insertInto);
    }
}

let isOnLoad = false;

async function load(url, insertInto){
    if (isOnLoad){
        return false;
    }

    isOnLoad = true;

    let response = await fetch(window.location.origin + url, {
        method: 'get',
        headers: {'Accept': 'application/json', "Content-Type": "application/json"}
    })

    console.log(response)

    if (response.ok){
        isOnLoad = false;

        let el = document.getElementById(insertInto);
        el.insertAdjacentHTML('beforeend', await response.text());
        
        return true;
    }

    return false;
}