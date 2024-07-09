function popMsg(message) {
	$("#message").html(message);
	$("#message_cover").css({display: 'flex'});
}

function statusMsg(message) {
	$("#status_message").html(message);
}

function explainControls(){
	$('#menu_area').addClass('explain');
}
function stopExplaining(){
	$('#menu_area').removeClass('explain');
}

$(document).ready(function() {

	if (message == "initial") {
		popMsg(```

			<h2>First load detected!</h2>
			<p>Hi! Welcome to Paneless! Let's help you fix Microsoft's various mistakes and rude design choices!</p>
			<p>Your current Windows settings have been saved to a backup file that you can revert to at any time by clickcking "Load prior state" and selecting the "[today's date]_initial_backup.json" file.</p>
			<p>Otherwise, feel free to make any of the changes you like and once you have the settings the way you like, click the "Save this!" button to set that as your preferred state. Windows settings may drift or change from your preferred state over time, but that can all be fixed with a single click of "Restore to Ideal" at any time!</p>
			<p>This message will be shown once, but if you ever forget what the controls do, just click the "Explain Controls" button!</p>
		```);
		explainControls();
	}

	$('#explain_this_button').click(function() {
		explainControls();
	});
	$('#explain_off_button').click(function() {
		stopExplaining();
	});

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
			if (data.message.activation_message)
				popMsg(data.message.activation_message);
		})
		.catch(error => console.error('Error:', error));
	});
	$(document).on('click', '#restartWinExplorer', function(){
		fetch('/restart_win_explorer', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
		})
		.then(response => response.json())
		.then(data => {
			console.log(data.message);
			if (data.message.status_message)
				// it's already open so just need to cahnge the emssage.
				$("#message").html(data.message.status_message);
		})
		.catch(error => console.error('Error:', error));
	});

	$("[data-fix-key='GroupBy']").find(".default_button").html("Open tool");


	$("#message_cover, #message_x").click(function(e) {
		// Stop propagation in the message and inner links (like restarting Windows Explorer shortcut) WEREN'T WORKING
		// So here's a workaround. Only close it if they clicked the message cover itself.
        if (e.target.id === 'message_cover' || e.target.id === 'message_x')
			$("#message_cover").css({display: 'none'});
	});

	function filterFixes() {
		var filter = $('#filter_input').val().toLowerCase();

        if (filter.length < 1) {
            $('#clear_filter_button').prop('disabled', true);
        } else {
            $('#clear_filter_button').prop('disabled', false);
        }
		$('.a_fix').each(function() {
			var text = $(this).find('h2, p, .tags_area').text().toLowerCase();
			if (text.indexOf(filter) > -1) {
				$(this).show();
			} else {
				$(this).hide();
			}
		});
	}

	$('#filter_input').on('keyup', function() {
		filterFixes();
	});

	$('#clear_filter_button').click(function() {
		if (!$(this).prop('disabled')) {
			$('#filter_input').val('');
			filterFixes();
		}
	});

	$('#snark_on_button').click(function() {
		$('#snark_on_button, .description').hide();
		$('#snark_off_button, .snark').show();
	});
	$('#snark_off_button').click(function() {
		$('#snark_on_button, .description').show();
		$('#snark_off_button, .snark').hide();
	});

	$('.saveable').click(function(){
		var menuPrefs = {};
		// If the button to turn on snark is visible, that means snark is OFF
		menuPrefs['snark'] = $('#snark_on_button').is(':visible') ? 0 : 1;
		menuPrefs['explain'] = $('#explain_this_button').is(':visible') ? 0 : 1;
		console.log(menuPrefs);

		fetch('/save_menu_prefs', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify({ menu_prefs: menuPrefs })
		})
		.then(response => response.json())
		.then(data => {
			console.log(data.message);
			if (data.message.status_message)
				statusMsg(data.message.status_message);
		})
		.catch(error => console.error('Error:', error));
	});

	window.addEventListener('scroll', function() {
		var fixedArea = document.getElementById('fixed_area');
		if (window.scrollY > fixedArea.offsetTop) {
			fixedArea.classList.add('fixed');
		} else {
			fixedArea.classList.remove('fixed');
		}
	});


})