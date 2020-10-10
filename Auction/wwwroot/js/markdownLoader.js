
document.getElementById('prev').addEventListener('click', loadMarkdown);

async function loadMarkdown(){
    let markdown = document.getElementById('markdown-editor').value;
    
    let response = await fetch(window.location.origin + "/Lot/RenderMarkdown", {
        method: 'post',
        body: JSON.stringify(markdown),
        headers: {
            'X-ANTI-FORGERY-TOKEN': document.getElementsByName("__RequestVerificationToken")[0].value,
            'Accept': 'application/json',
            "Content-Type": "application/json",
        }
    })

    if (response.ok){
        document.getElementById('story').innerHTML = await response.text();
    }
}