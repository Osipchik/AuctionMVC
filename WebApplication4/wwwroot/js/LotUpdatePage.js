let input = document.getElementById('min-price-input');
let value = document.getElementById('min-price-hidden');
console.log(value.innerText)
input.value = value.innerText.split(',')[0];
// input.value = value.innerText;

// input.addEventListener('input', (e) => {
//     input.value = input.value.replace(/[^0-9,-]/g, '')
//    
//     let regexp = /^\$?[0-9]*[0-9]\.?[0-9]{0,2}$/i;
//     if(!regexp.test(input.value)){
//         let temp = input.value.split('.');
//         if(temp[1]){
//             temp[1] = temp[1]?.substr(0, 2);
//             input.value = temp.join('.');
//         }
//     }
//
//     console.log(input.value);
// });