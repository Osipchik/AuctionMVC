// "use strict"

console.log(123)

document.getElementById("post-comment-button").disabled = true;
const lotId = document.getElementById('Id').value;

const connection = new signalR.HubConnectionBuilder().withUrl('/commentsHub').build();

connection.start().then(function () {
    document.getElementById("post-comment-button").disabled = false;
    
    // connection.invoke("JoinRoom", lotId).catch(function (err) {
    //     return console.error(err.toString());
    // })
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById('post-comment-button')
    .addEventListener('click', postComment);

function postComment(){
    let data = {
        message: document.getElementById("comment-content").value,
        lotId: lotId
    }
    
    console.log(data)
    // connection.invoke("PostComment", data).catch(function (err) {
    //     return console.error(err.toString());
    // });
}

connection.on("ReceiveComment", function (message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    console.log(msg)
    // let encodedMsg = user + " says " + msg;
    // let li = document.createElement("li");
    // li.textContent = encodedMsg;
    // document.getElementById("comments-list").appendChild(li);
});