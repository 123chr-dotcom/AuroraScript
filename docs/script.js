// 文档加载完成后执行
document.addEventListener('DOMContentLoaded', function() {
    // 高亮当前页面的导航链接
    highlightCurrentPage();
    
    // 添加返回顶部按钮
    addBackToTopButton();
});

/**
 * 高亮当前页面的导航链接
 */
function highlightCurrentPage() {
    const navLinks = document.querySelectorAll('nav a');
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPage) {
            link.classList.add('active');
        }
    });
}

/**
 * 添加返回顶部按钮
 */
function addBackToTopButton() {
    const button = document.createElement('button');
    button.id = 'back-to-top';
    button.innerHTML = '↑';
    button.title = '返回顶部';
    button.style.display = 'none';
    button.style.position = 'fixed';
    button.style.bottom = '20px';
    button.style.right = '20px';
    button.style.zIndex = '99';
    button.style.border = 'none';
    button.style.outline = 'none';
    button.style.backgroundColor = 'var(--primary-color)';
    button.style.color = 'white';
    button.style.cursor = 'pointer';
    button.style.padding = '15px';
    button.style.borderRadius = '50%';
    button.style.fontSize = '18px';
    
    document.body.appendChild(button);
    
    // 显示/隐藏按钮
    window.addEventListener('scroll', function() {
        if (window.pageYOffset > 300) {
            button.style.display = 'block';
        } else {
            button.style.display = 'none';
        }
    });
    
    // 点击返回顶部
    button.addEventListener('click', function() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
}
