// EzChat 클라이언트 스크립트

// DOM 로드 완료 후 실행
document.addEventListener('DOMContentLoaded', function() {
    // 알림 자동 숨김
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            alert.style.opacity = '0';
            setTimeout(function() {
                alert.remove();
            }, 300);
        }, 5000);
    });
});
