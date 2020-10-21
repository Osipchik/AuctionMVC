"use strict"

// document.getElementById("post-comment-button").disabled = true;
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

    let response = await fetch(window.location.origin + "/Comment/GetPage?page=1", {
        method: 'get'
    })
    
    if (response.ok){
        isLoaded = true;
        let a = await response.text();
        console.log(a);
        document.getElementById('comments').innerHTML = a;

        connect();
    }
}

function connect(){
    connection.invoke("JoinRoom", lotId)
        .catch(function (err) {
            return console.error(err.toString());
        })

    document.getElementById('post-comment-button')
        .addEventListener('click', postComment);
}

function postComment(){
    let data = {
        message: document.getElementById("comment-content").value,
        lotId: lotId
    }
    
    console.log(data)

    connection.invoke("PostComment", data).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("ReceiveComment", function (message) {
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