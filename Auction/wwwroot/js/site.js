
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