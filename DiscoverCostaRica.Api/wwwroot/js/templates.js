const templates = {
    renderEndpoint(endpoint) {
        const methodClass = endpoint.method.toLowerCase();
        let html = `
            <div class="endpoint-card">
                <div class="endpoint-header">
                    <span class="method ${methodClass}">${endpoint.method}</span>
                    <span class="endpoint-path">${endpoint.path}</span>
                </div>
                <p>${endpoint.description}</p>
        `;

        if (endpoint.request) {
            html += `
                <h4>Request:</h4>
                <pre><code class="language-json">${JSON.stringify(endpoint.request, null, 2)}</code></pre>
            `;
        }

        if (endpoint.response) {
            html += `
                <h4>Response:</h4>
                <pre><code class="language-json">${JSON.stringify(endpoint.response, null, 2)}</code></pre>
            `;
        }

        html += '</div>';
        return html;
    },

    renderModel(model) {
        return `
            <div class="model-card">
                <h3>${model.name}</h3>
                <p>${model.description}</p>
                <pre><code class="language-json">${JSON.stringify(model.schema, null, 2)}</code></pre>
            </div>
        `;
    },

    renderEndpointsPage() {
        let html = '<h1>API Endpoints</h1>';
        
        // Users section
        html += '<h2 class="section-title">Users</h2>';
        apiData.endpoints.users.forEach(endpoint => {
            html += this.renderEndpoint(endpoint);
        });

        // Auth section
        html += '<h2 class="section-title">Authentication</h2>';
        apiData.endpoints.auth.forEach(endpoint => {
            html += this.renderEndpoint(endpoint);
        });

        return html;
    },

    renderModelsPage() {
        let html = '<h1>API Models</h1>';
        apiData.models.forEach(model => {
            html += this.renderModel(model);
        });
        return html;
    }
};