let take = 10;
let skip = 0;

const insertInto = 'user-lots-list';
function getUrl(){
    let userName = document.getElementById('user-name').innerText;
    return `/Home/LoadLots?search=@${userName}&sortBy=1&show=3&take=${take}&skip=${skip}`;
}

async function loadNumber(){
    if (await load(getUrl(), insertInto)){
        skip += take;
        console.log('users load')
    }
}

window.addEventListener('scroll', async () => await loadByMark(loadNumber));
document.addEventListener("DOMContentLoaded", loadNumber);