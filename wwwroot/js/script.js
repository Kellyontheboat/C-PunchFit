const apiBaseUrl = 'https://localhost:7253/api/Counter';

const counterSpan = document.getElementById('counter');
const incrementButton = document.getElementById('incrementButton');

//// 初始化時載入目前數字
//async function loadCounter() {
//    const response = await fetch(apiBaseUrl);
//    const data = await response.json();
//    counterSpan.textContent = data.counter;
//}

//// 點擊按鈕後遞增數字
//incrementButton.addEventListener('click', async () => {
//    const response = await fetch(apiBaseUrl, { method: 'POST' });
//    const data = await response.json();
//    counterSpan.textContent = data.counter;
//});

//// 執行初始化
/*loadCounter();*/