"use strict"

let loadParams = {
    show: '',
    sort: '',
    search: '',
    categoryId: ''
}

let take = 30;
let skip = 0;

const insertInto = 'lot-view-container';
function getUrl(){
    let url = '/Home/LoadLots/?' +
        `search=${loadParams.search}&` +
        `Category=${loadParams.categoryId}&` +
        `Show=${loadParams.show}&` +
        `SortBy=${loadParams.sort}&` +
        `take=${take}&skip=${skip}`;
        
    return url;
}

document.querySelectorAll('[data-filter]')
    .forEach(i => i.addEventListener('click', onChange));

function onChange(e){
    e.preventDefault();
    console.log(123);
    
    loadParams.show = document.getElementById('show-filter').value;
    loadParams.sort = document.getElementById('sort-filter').value;
    loadParams.search = document.getElementById('search-input').value;
    loadParams.categoryId = document.getElementById('category-filter').value;

    take = 10;
    skip = 0;

    document.getElementById(insertInto).innerHTML = '';
    
    load(getUrl(), insertInto);
}

async function loadNumber(){
    if (await load(getUrl(), insertInto)){
        skip += take;
    }
}

window.addEventListener('scroll', async () => await loadByMark(loadNumber));
// document.addEventListener("DOMContentLoaded", loadNumber);
loadNumber();