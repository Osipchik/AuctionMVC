let take = 10;
let skip = 0;

const insertInto = 'users-list';
function getUrl(){
    return `/Admin/GetUsers?take=${take}&skip=${skip}`;
}

async function loadNumber(){
    if (await load(getUrl(), insertInto)){
        skip += take;
        console.log('admin load')
    }
}

window.addEventListener('scroll', async () => await loadByMark(loadNumber));
document.addEventListener("DOMContentLoaded", loadNumber);
