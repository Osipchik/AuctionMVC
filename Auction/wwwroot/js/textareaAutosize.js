let textarea = document.querySelectorAll('textarea');

textarea.forEach(i => {
    i.addEventListener('keydown', autosize);
    setSize(i);
});

function autosize(){
    setTimeout(function(){
        setSize(this)
    },0);
}


function setSize(element){
    element.style.cssText = 'height:auto; padding:0';
    element.style.cssText = 'height:' + (element.scrollHeight + 2) + 'px';
}