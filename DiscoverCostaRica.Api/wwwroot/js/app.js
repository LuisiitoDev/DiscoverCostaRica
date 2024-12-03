document.addEventListener('DOMContentLoaded', () => {
    // Initialize with dashboard view
    const dashboardLink = document.querySelector('a[href="#dashboard"]');
    dashboardLink.addEventListener('click', (e) => {
        e.preventDefault();
        showContent('dashboard');
        
        // Update active states
        document.querySelectorAll('.nav a').forEach(link => link.classList.remove('active'));
        dashboardLink.classList.add('active');
    });
});