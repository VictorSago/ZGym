// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let p = document.querySelector('#create-ajax');
// document.querySelector('#fetch').addEventListener('click', callBack);

async function callBack() {
    fetch('https://localhost:5001/GymClasses/Fetch', {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(res => res.text())
      .then(data => {
          p.innerHTML = data;
      })
      .catch(err => console.log(err));
}

$('#fetch').click(function () {
    $.ajax({
        url: 'https://localhost:5001/GymClasses/Fetch',
        type: 'GET',
        success: success,
        failure: fail
    });
})

function success(response) {
    if (200 == response.status) {
        console.log('OK')
    }
    p.innerHTML = response;
}

function removeForm() {
    $('#create-ajax').remove();
}

function fail() {
    console.log("Failed in creation");
}

$('#history-checkbox').click(function() {
    $('#history-form').submit();
})