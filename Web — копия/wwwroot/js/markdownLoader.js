"use strict"


document.getElementById('prev').addEventListener('click', loadMarkdown);

async function loadMarkdown(){
    let markdown = document.getElementById('markdown-editor').value;
    
    let response = await fetch(window.location.origin + "/Lot/RenderMarkdown", {
        method: 'post',
        body: JSON.stringify(markdown),
        headers: getFormHeader()
    })
    
    if (response.ok){
        document.getElementById('story').innerHTML = await response.text();
    }
}