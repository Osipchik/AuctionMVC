"use strict"

document.getElementById("post-comment-button").disabled = true;

const connection = new signalR.HubConnectionBuilder().withUrl('/commentsHub').build();

connection.start().then(function () {
    document.getElementById("post-comment-button").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById('post-comment-button')
    .addEventListener('click', postComment);

function postComment(){
    let lotId = document.getElementById('Id').value;
    let message = document.getElementById("comment-content").value;
    
    connection.invoke("PostComment", message).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("ReceiveComment", function (user, message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = user + " says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("comments-list").appendChild(li);
});