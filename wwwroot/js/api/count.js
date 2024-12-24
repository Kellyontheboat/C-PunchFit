const apiBaseUrl = 'https://localhost:7253/api/Counter';


const counterSpan = document.getElementById('counter');
const incrementButton = document.getElementById('incrementButton');
const resetButton = document.getElementById('resetButton');
console.log(resetButton)

// 初始化時載入目前數字
async function loadCounter() {
    const response = await fetch(apiBaseUrl);
    const data = await response.json();
    counterSpan.textContent = data.counter;
}

// 點擊按鈕後遞增數字
incrementButton.addEventListener('click', async () => {
    const response = await fetch(apiBaseUrl, { method: 'POST' });
    const data = await response.json();
    counterSpan.textContent = data.counter;
});

// 點擊按鈕後歸零數字
resetButton.addEventListener('click', () => {
    console.log("reset")
    counterSpan.textContent = 0;
});


// Login function
async function login(event) {
    event.preventDefault(); // Prevent form from submitting the default way

    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    const formData = new FormData();
    formData.append('Email', email);
    formData.append('Password', password);

    const response = await fetch('https://localhost:7253/api/login', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
        const data = await response.json();

        localStorage.setItem('authToken', data.token);
        alert('Login successful!');
    } else {
        alert('Login failed. Please check your credentials.');
    }
}

// Attach login function to form submit event
const loginForm = document.getElementById('loginForm');
loginForm.addEventListener('submit', login);

// Verify token function
async function verifyToken() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert('No token found. Please log in.');
        return false;
    }

    console.log(token)
    const response = await fetch('https://localhost:7253/api/verify-token', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(token)
    });

    if (response.ok) {
        const claims = await response.json();
        console.log('Token is valid:', claims);
        return true;
    } else {
        alert('Invalid token. Please log in again.');
        return false;
    }
}

async function getExercisesGraphQL() {

}

async function getModuleWithItemsGraphQL() {

    console.log('getModuleWithItems function start verify!!')
    const isTokenValid = await verifyToken();
    if (!isTokenValid) {
        return;
    }
        const startTime = performance.now(); // Start time

        console.log('getModuleWithItems query start!!')
        const query = `
            query GetModulesWithItems {
                modules {
                    id
                    member_id
                    created_at
                    section_id
                    module_name
                    moduleItems {
                        id
                        exercise_id
                        module_id
                        reps
                        sets
                        weight
                        }
                 }
            }
        `;

        const response = await fetch('https://localhost:7253/graphql', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({ query })
        });

        const responseText = await response.text();
        console.log('Response Text:', responseText);

        try {
            const resultModulesItems = JSON.parse(responseText);
            console.log("GraphQL",resultModulesItems); // Output the fetched Modules and Items

            const endTime = performance.now(); // End time
            console.log(`getModuleWithItemsGraphQL took ${endTime - startTime} milliseconds.`);

            return resultModulesItems.data;
        } catch (error) {
            console.error('Error parsing JSON:', error);
        }
}

// Fetch Modules and their ModuleItems using REST API
async function fetchModulesWithItemsRest() {
    console.log('Fetching ModulesWithItems using REST API');
    const isTokenValid = await verifyToken();
    if (!isTokenValid) {
        return;
    }

    const startTime = performance.now(); // Start time

    const modulesResponse = await fetch('https://localhost:7253/api/Modules', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
    });

    if (modulesResponse.ok) {
        const modules = await modulesResponse.json();
        const modulesWithItems = [];

        for (const module of modules.$values) {
            const moduleItemsResponse = await fetch(`https://localhost:7253/api/Modules/${module.id}/ModuleItems`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });

            if (moduleItemsResponse.ok) {
                const moduleItems = await moduleItemsResponse.json();
                modulesWithItems.push({ ...module, moduleItems });
            } else {
                console.error(`Failed to fetch ModuleItems for Module ${module.id}`);
            }
        }

        console.log("Rest",modulesWithItems); // Output the fetched Modules and Items
        const endTime = performance.now(); // End time
        console.log(`fetchModulesWithItemsRest took ${endTime - startTime} milliseconds.`);

        return modulesWithItems;
    } else {
        console.error('Failed to fetch Modules');
    }
}

getModuleWithItemsGraphQL()
fetchModulesWithItemsRest()

//// 登入資訊
//const function retrieveUserData();
//const userDataResponse = await fetch("https://localhost:7253/api/login");
//const userData = await userDataResponse.json();
//console.log(userData);

//加入graphql api

////get ModuleItems
//async function getModuleItems(moduleId) {
//    const query = `
//        query GetModuleItems($moduleId: Int!) {
//            moduleItems(moduleId: $moduleId) {
//                id
//                exercise_id
//                module_id

//            }
//        }
//    `;
//    const variables = { moduleId: moduleId };

//    const response = await fetch('https://localhost:7253/graphql', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json',
//        },
//        body: JSON.stringify({ query, variables }),
//    });
//}

////get Modules
//async function getModules(memberId) {
//    const query = `
//    query getModules(memberId: Int!) {
//        modules(memberId: $memberId) {

//            module_id

//        }
//    }
//`;
//    const memberVariables = { memberId: memberId };

//    const response = await fetch('https://localhost:7253/graphql', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json',
//        },
//        body: JSON.stringify({ query, memberVariables }),
//    });

//    const resultModules = await response.json();
//    console.log(resultModules); // Output the fetched Modules
//    return resultModules.data;
//}

//// Fetch ModulesWithItems using REST API
//async function fetchModulesWithItemsRest() {
//    console.log('Fetching ModulesWithItems using REST API');
//    const isTokenValid = await verifyToken();
//    if (!isTokenValid) {
//        return;
//    }

//    const response = await fetch('https://localhost:7253/api/restful/ModulesWithItems', {
//        method: 'GET',
//        headers: {
//            'Content-Type': 'application/json',
//            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
//        }
//    });

//    if (response.ok) {
//        const resultModulesItems = await response.json();
//        console.log('ModulesWithItems (REST):', resultModulesItems);
//        return resultModulesItems;
//    } else {
//        console.error('Failed to fetch ModulesWithItems using REST API');
//    }
//}


//getModuleItems(6)
//getModules(2)