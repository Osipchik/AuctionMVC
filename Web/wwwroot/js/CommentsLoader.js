"use strict"

document.getElementById('comments-link')
    .addEventListener('click', async () => await loadComments());

let isLoaded = false
const connection = new signalR.HubConnectionBuilder().withUrl('/commentsHub').build();

connection.start().then(function () {
    document.getElementById("post-comment-button").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

async function loadComments(){
    console.log(isLoaded)
    if (isLoaded){
        return
    }

    if (await load()){
        connect();   
    }
}

function connect(){
    connection.invoke("JoinRoom", lotId)
        .catch(function (err) {
            return console.error(err.toString());
        })

    console.log('connect');
    document.getElementById('post-comment-button')
        .addEventListener('click', postComment);
}

function postComment(){
    let data = {
        message: document.getElementById("comment-content").value,
        lotId: lotId
    }

    document.getElementById("comment-content").value = '';
    console.log(data)

    connection.invoke("PostComment", data).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("ReceiveComment", function (message) {
    skip += 1;
    
    let msg = message.text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    let li = document.createElement("li");
    li.classList.add('media', 'my-2');

    let div = document.createElement("div");
    div.classList.add('media-body');

    let h = document.createElement("h5");
    h.classList.add('mt-0',  'mb-1');
    h.textContent = message.appUser.userName;
    
    div.textContent = msg;
    div.prepend(h)
    
    li.appendChild(div);
    
    document.getElementById("comments-list").prepend(li);
});


window.addEventListener('scroll', loadByMark)

let take = 10;
let skip = 0;
let isOnLoad = false;

async function load(){
    if (isOnLoad){
        return
    }
    
    isOnLoad = true;
    
    let urlRequest = `/Comment/GetComments?lotId=${lotId}&take=${take}&skip=${skip}`;
    let response = await fetch(window.location.origin + urlRequest, {
        method: 'get',
        headers: {'Accept': 'application/json', "Content-Type": "application/json"}
    })
    
    if (response.ok){
        isOnLoad = false;
        skip += take;
        
        let el = document.createElement('div');
        el.innerHTML = await response.text();
        document.getElementById('comments').appendChild(el);
        
        return true;
    }
    
    return false;
}


async function loadByMark(){
    let mark = document.getElementById('last-comment');
    if (mark !== null && mark.style['display'] !== 'none'){
        mark.remove();
        console.log(123)
        await load();
    }
}