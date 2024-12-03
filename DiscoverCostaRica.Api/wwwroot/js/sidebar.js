document.addEventListener('DOMContentLoaded', () => {
    const sidebarLinks = document.querySelectorAll('.sidebar .nav > li > a');
    
    sidebarLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            if (link.getAttribute('data-section')) {
                e.preventDefault();
                const submenu = link.nextElementSibling;
                submenu.classList.toggle('active');
                link.classList.toggle('active');
            }
        });
    });

    // Handle submenu item clicks
    const submenuLinks = document.querySelectorAll('.submenu a');
    submenuLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const targetId = link.getAttribute('href').substring(1);
            showContent(targetId);
            
            // Update active states
            submenuLinks.forEach(l => l.classList.remove('active'));
            link.classList.add('active');
        });
    });
});