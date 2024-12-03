class Navigation {
    constructor() {
        this.pages = document.querySelectorAll('.page');
        this.links = document.querySelectorAll('.sidebar a');
        this.init();
    }

    init() {
        this.links.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const pageId = link.getAttribute('data-page');
                this.showPage(pageId);
            });
        });
    }

    showPage(pageId) {
        // Update active page
        this.pages.forEach(page => {
            page.classList.remove('active');
            if (page.id === pageId) {
                page.classList.add('active');
            }
        });

        // Update active link
        this.links.forEach(link => {
            link.classList.remove('active');
            if (link.getAttribute('data-page') === pageId) {
                link.classList.add('active');
            }
        });
    }
}