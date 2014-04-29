jQuery.fn.collapse = function(options) {
	var defaults = {
		closed : false
	}
	settings = jQuery.extend({}, defaults, options);

	last = null;
	return this.each(function() {
		var obj = jQuery(this);
		obj.find("legend:first").addClass('collapsible').click(function() {
			if(last && last != obj) {
				last.find("legend:first").click();
			}
			if (obj.hasClass('collapsed')) {
				obj.removeClass('collapsed').addClass('collapsible');
			}
	
			jQuery(this).removeClass('collapsed');
	
			obj.children().not('legend').toggle("fast", function() {
				 if (jQuery(this).is(":visible")) {
					obj.find("legend:first").addClass('collapsible');
					obj.find("input[type=text]:first").focus();
					last = obj;
				 } else {
				 	if(last == obj) { last = null; }
					obj.addClass('collapsed').find("legend").addClass('collapsed');
				 }
			 });
		});
		if (settings.closed) {
			obj.addClass('collapsed').find("legend:first").addClass('collapsed');
			obj.children().not("legend:first").css('display', 'none');
		}
	});
};


$(function(){
	$(".menu .children").hide();
	$(".menu .parent").each(function() {
		if(document.URL.indexOf("/" + $(this).children("a:first").attr("data-module") + "-") > -1) {
			$(this).children(".children").show();
		}
	});
	$(".menu .parent").mouseenter(function() {
		$(".menu .children").hide();
		$(this).children(".children").show();
	});
	$("form.operation-form fieldset").collapse({closed:true});
	$("td.action a.reference-link").button();
	$("table").each(function (){
		var tbl = $(this)
	    var obj = $.paramquery.tableToArray(tbl);
	    var grid = $("<div />");
	    tbl.after(grid);
	    grid.pqGrid({
	    	width:937, height:400, 
	    	topVisible:false, numberCell:false, 
	    	scrollModel:{horizontal:false}, 
	    	editable:false, resizable:true,
	    	hoverMode:"none", 
	    	dataModel:{ data:obj.data, rPP:10, paging:"local" }, 
	    	colModel:obj.colModel 
	    });
	    tbl.css("display", "none");
	});
	$(".tabs").tabs();
	$('input[data-type="s-boolean"][type="checkbox"]').change(function() {
		$('input[name="' + $(this).attr("name") + '"][type="hidden"]').attr("disabled", $(this).is(":checked"));
	});
	$('input[data-type="s-date-time"]').datepicker();
	$('input[data-type="s-decimal"]').numeric({decimal:".",negative:false});
	$('input[data-type="s-double"]').numeric({decimal:"."});
	$('input[data-type="s-int-32"]').numeric({decimal:" "});
	$('input[type=submit].needs-confirm').each(function(){
		$(this).on({click:function(){
			confirmDialog("Are you sure?", $(this).val(), $(this).parents("form:first"));
			return false;	
		}});
	});
});

function confirmDialog(message, okText, form) {
	var buttons = new Object();
	buttons[okText] = function() { $(this).dialog("close"); $(this).dialog("destroy"); form.submit(); };
	buttons["Cancel"] = function() { $(this).dialog("close"); $(this).dialog("destroy"); };
    $('<div>' + message + '</div>').dialog({
        modal: true,
        title: "Confirmation",
        buttons : buttons
    });
}
