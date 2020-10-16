async function onChange(){
    let show = document.getElementById('show-filter').value;
    let sort = document.getElementById('sort-filter').value;
    let search = document.getElementById('search-input').value;
    
    let url = `/?search=${search}&Show=${show}&SortBy=${sort}&page=1`;

    await fetchView(url)
}

async function onPageClick(){
    await fetchView(this.dataset.href)
}


async function fetchView(url){
    
    let response = await fetch(window.location.origin + '/home/GetPage' + url);
    if (response.ok){
        changeUrl(url)
        document.querySelectorAll('[data-href]')
            .forEach(i => i.removeEventListener('click', onPageClick))

        document.getElementById('lot-view-container').innerHTML = await response.text();

        document.querySelectorAll('[data-href]')
            .forEach(i => i.addEventListener('click', onPageClick))
        
        document.getElementById('items-count').innerHTML =
            document.getElementById('PagingInfo_TotalPages').value;
    }
}

document.querySelectorAll('[data-href]')
    .forEach(i => i.addEventListener('click', onPageClick))

