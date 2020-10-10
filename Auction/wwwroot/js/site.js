
function changeUrl(newUrl){
    if (history.pushState) {
        window.history.pushState('', '', newUrl);
    } 
    else {
        document.location.href = newUrl;
    }
}


