﻿"use strict"

document.getElementById('comments-link')
    ?.addEventListener('click', async () => await loadComments());

document.getElementById('post-comment-button')
    ?.addEventListener('click', postComment);


let isLoaded = false
async function loadComments(){
    if (!isLoaded){
        await load();
        
        let loadInterval = setTimeout(async function loadFunc(){
            await load();
            loadInterval = setTimeout(loadFunc, 2000)
        }, 2000);
    }
}

async function postComment(e){
    e.preventDefault();
    let urlParams = `lotId=${lotId}&message=${document.getElementById("comment-content").value}`;
    let response = await fetch(window.location.origin + '/Comment/PostComment?' + urlParams, {
        method: 'post',
        headers: getFormHeader()
    })

    console.log(response)
    if(response.ok){
        document.getElementById("comment-content").value = '';
        document.getElementById("comments-list").innerHTML = await response.text();
    }
}

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
    console.log('load')
    if (response.ok){
        isOnLoad = false;
        skip += take;
        
        let el = document.getElementById('comments-list');
        el.insertAdjacentHTML('beforeend', await response.text());
        el.querySelectorAll('[data-delete-comment]').forEach(i => {
            i.addEventListener('click', onDeleteCommentClick)
        });
        
        return true;
    }
    
    return false;
}


async function loadByMark(){
    let mark = document.getElementById('last-comment');
    if (mark !== null && mark.style['display'] !== 'none'){
        mark.remove();
        await load();
    }
}


async function onDeleteCommentClick(e){
    e.preventDefault();
    let commentId = e.target.closest('div').dataset.deleteComment;
    
    let url = '/Comment/DeleteComment?commentId=' + commentId;
    let response = await fetch(window.location.origin + url, {
        method: 'delete',
        headers: getFormHeader()
    });

    if (response.ok){
        let commentElement = document.getElementById(`comment-${commentId}`);
        commentElement.remove();
    }
}