const apiData = {
    endpoints: {
        users: [
            {
                method: 'GET',
                path: '/api/users',
                description: 'Retrieve a list of users',
                response: {
                    users: [
                        {
                            id: 'string',
                            username: 'string',
                            email: 'string'
                        }
                    ]
                }
            },
            {
                method: 'POST',
                path: '/api/users',
                description: 'Create a new user',
                request: {
                    username: 'string',
                    email: 'string',
                    password: 'string'
                },
                response: {
                    id: 'string',
                    username: 'string',
                    email: 'string',
                    createdAt: 'datetime'
                }
            },
            {
                method: 'PUT',
                path: '/api/users/{id}',
                description: 'Update an existing user',
                request: {
                    username: 'string',
                    email: 'string'
                },
                response: {
                    id: 'string',
                    username: 'string',
                    email: 'string',
                    updatedAt: 'datetime'
                }
            },
            {
                method: 'DELETE',
                path: '/api/users/{id}',
                description: 'Delete a user'
            }
        ],
        auth: [
            {
                method: 'POST',
                path: '/api/auth/login',
                description: 'Authenticate user and receive access token',
                request: {
                    email: 'string',
                    password: 'string'
                },
                response: {
                    accessToken: 'string',
                    refreshToken: 'string',
                    expiresIn: 3600
                }
            },
            {
                method: 'POST',
                path: '/api/auth/refresh',
                description: 'Refresh access token',
                request: {
                    refreshToken: 'string'
                },
                response: {
                    accessToken: 'string',
                    refreshToken: 'string',
                    expiresIn: 3600
                }
            }
        ]
    },
    models: [
        {
            name: 'User',
            description: 'Represents a user in the system',
            schema: {
                id: 'string',
                username: 'string',
                email: 'string',
                createdAt: 'datetime',
                updatedAt: 'datetime'
            }
        },
        {
            name: 'AuthResponse',
            description: 'Authentication response containing tokens',
            schema: {
                accessToken: 'string',
                refreshToken: 'string',
                expiresIn: 'number'
            }
        }
    ]
};