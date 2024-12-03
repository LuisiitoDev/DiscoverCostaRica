const endpointData = {
    beaches: {
        title: 'Beaches',
        method: 'GET',
        path: '/api/v1/Beaches',
        description: 'Get a list of all beaches',
        response: [
            {
                id: 1,
                name: 'Beach Name',
                description: 'Description'
            }
        ]
    },
    country: {
        title: 'Country',
        method: 'GET',
        path: '/api/v1/Country',
        description: 'Get country information',
        response: {
            id: 1,
            name: 'Country Name',
            description: 'Country Description',
            countryCode: 'Country code',
            population: 'Country population',
            location: 'Country location',

        }
    },
    province: {
        title: 'Province',
        method: 'GET',
        path: '/api/v1/Provinces',
        description: 'Get all provinces',
        response: [
            {
                id: 1,
                name: 'Province Name'
            }
        ]
    },
    cantons: {
        title: 'Canton',
        method: 'GET',
        path: '/api/v1/{{provinceId}}/Cantons',
        description: 'Get all cantons',
        response: [
            {
                id: 1,
                name: 'Canton Name'
            }
        ]
    },
    districts: {
        title: 'District',
        method: 'GET',
        path: '/api/v1/{{provinceId}}/{{cantonId}}/Districts',
        description: 'Get all districts',
        response: [
            {
                id: 1,
                name: 'District Name'
            }
        ]
    },
    volcanos: {
        title: 'Volcano',
        method: 'GET',
        path: '/api/v1/Volcanos',
        description: 'Get all volcanos',
        response: [
            {
                id: 1,
                name: 'Volcano Name',
                description: 'Description'
            }
        ]
    },
    dish: {
        title: 'Dish',
        method: 'GET',
        path: '/api/v1/Dish',
        description: 'Get all dishes',
        response: [
            {
                id: 1,
                name: 'Dish Name',
                description: 'Description'
            }
        ]
    }
};

function showContent(sectionId) {
    // Hide all content sections
    document.querySelectorAll('.content-section').forEach(section => {
        section.classList.remove('active');
    });

    // Show selected section
    const section = document.getElementById(sectionId);
    if (section) {
        section.classList.add('active');
    } else {
        // Create new section if it doesn't exist
        createEndpointSection(sectionId);
    }
}

function createEndpointSection(sectionId) {
    const data = endpointData[sectionId];
    if (!data) return;

    const section = document.createElement('div');
    section.id = sectionId;
    section.className = 'content-section';
    section.innerHTML = `
        <div class="endpoint-card">
            <div class="endpoint-header">
                <span class="endpoint-method method get">${data.method}</span>
                <span class="endpoint-path">${data.path}</span>
            </div>
            <div class="endpoint-description">${data.description}</div>
            <div class="response-example">
                <pre><code>${JSON.stringify(data.response, null, 2)}</code></pre>
            </div>
        </div>
    `;

    document.querySelector('.content-wrapper').appendChild(section);
    section.classList.add('active');
}