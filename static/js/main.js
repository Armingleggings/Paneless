$(document).ready(function() {
	$('.a_fix h2').click(function() {
		var fixBlock = this;
		const fixKey = $(fixBlock).data('fixKey');
		const fixed = $(fixBlock).data('fixed');
		fetch('/toggle_fix', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ fix_key: fixKey, fixed: fixed })
		})
		.then(response => response.json())
		.then(data => {
			console.log(data.message);
			$(fixBlock).find('.button').removeClass('on');
			if (data.message.fixed) {
				$(fixBlock).find('.fixed_button').addClass('on');
				$(fixBlock).data('fixed',1);
			} else {
				$(fixBlock).find('.default_button').addClass('on');
				$(fixBlock).data('fixed',0);
			}
		})
		.catch(error => console.error('Error:', error));
	});
})

/*
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.a_fix h2').forEach(button => {
        button.addEventListener('click', function() {
            const fixKey = this.dataset.fixKey;
            const fixed = this.dataset.fixed;
            fetch('/toggle_fix', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ fix_key: fixKey, fixed: fixed })
            })
            .then(response => response.json())
            .then(data => {
                console.log(data.message);
                const buttonElements = this.parentNode.querySelectorAll('.button');
                buttonElements.forEach(button => button.classList.remove('on'));
                if (data.message.fixed) {
                    this.parentNode.querySelector('.fixed_button').classList.add('on');
                    this.parentNode.querySelector('.fixed_button').classList.add('on');
                } else {
                    this.parentNode.querySelector('.default_button').classList.add('on');
                }
                // Optionally update the UI based on the response
            })
            .catch(error => console.error('Error:', error));
        });
    });
});
*/