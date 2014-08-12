
function removeResultList(resultContainer, searchinput){
	if($("#"+searchinput).val().length==0){
		$("#"+resultContainer).html("");
	}
}
/*
VOTE
*/

function addVote(idposte){
		var ajaxurl = "/ensc-web/addvotetopost/" + idposte;
		updateVoteAjax(ajaxurl,idposte);
	}
	function deleteVote(idposte){
		var ajaxurl = "/ensc-web/deletevotetopost/" + idposte;
		updateVoteAjax(ajaxurl,idposte);
	}

	function updateVoteAjax(ajaxurl,idposte){
		$.ajax({
	        url: ajaxurl,
	        type:"GET",
	        dataType:'json',
	        async: true,
        	cache: true,
	        success: function( data ) {
              $("#nbVotes"+idposte).text(data.nbvotes);
              $("#delVoteBtn"+idposte).attr("disabled","disabled");
              $("#addVoteBtn"+idposte).attr("disabled","disabled");
         	}});
	}

	function saveQuestion(idPost,idUser){
		var ajaxurl = "/ensc-web/tests/savequestion/"+idPost+"/"+idUser;
		$.ajax({
	        url: ajaxurl,
	        type:"GET",
	        dataType:'json',
	        async: true,
        	cache: true,
	        success: function( data ) {
              $("#saveButton").attr("disabled","disabled");
         	}}); 	
	}





function searchajax(resultContainer, searchinput){	
	var ajaxurl = "/ensc-web/searchtextajax";
	if($("#"+searchinput).val().length>0){
		$.ajax({
	        url: ajaxurl,
	        type:"POST",
	        dataType:'html',
	        async: true,
	    	cache: true,
	    	data: {"search":$("#"+searchinput).val()
	    	},
	        success: function( data ) {
	        	$("#"+resultContainer).html(data);
	     	},
	     	error: function( xhr, status ) {
	        //alert( "Sorry, there was a problem!" );
	        },
	        complete: function( xhr, status ) {
	            //$('#showresults').slideDown('slow')
	        }
		});
	}
}
if($("#searchinput").length){
	$("#searchinput").on('keydown', function() {
	    var key = event.keyCode || event.charCode;
	    if( key == 8 || key == 46 ){
	        searchajax("resultContainer", "searchinput");
		    if($("#searchinput").val().length==1){
				$("#results").html("");
				//$("#trending").removeClass("hide");	
			}
		}});



	if($("#searchinput").val().length>0){
		searchajax("results", "searchinput");
	}
}
function searchajax(resultContainer, searchinput){	
	var ajaxurl = "/ensc-web/searchtextajax";
	if($("#"+searchinput).val().length>0){
		$.ajax({
	        url: ajaxurl,
	        type:"POST",
	        dataType:'html',
	        async: true,
        	cache: true,
        	data: {"search":$("#"+searchinput).val()
        	},
	        success: function( data ) {
	        	$("#"+resultContainer).html(data);
	        	//$("#trending").addClass("hide");	
         	},
         	error: function( xhr, status ) {
            //alert( "Sorry, there was a problem!" );
            },
            complete: function( xhr, status ) {
                //$('#showresults').slideDown('slow')
            }
		});
	}
}

/* 
TAGS
*/

$('#title').bind("keyup keypress", function(e) {
  var code = e.keyCode || e.which; 
  if (code  == 13) {               
    e.preventDefault();
    return false;
  }
});
	$("#tagField").autocomplete({
		source:function(req, add) {
		var ajaxurl = "/ensc-web/ajaxTagsStartingWith/" + $("#tagField").val();
		if($("#tagField").val().length>0){
		 	$.ajax({
		        url: ajaxurl,
		        type:"GET",
		        dataType:'json',
		        async: true,
            	cache: true,
		        success: function( data ) {
		        	var suggestions = [];  
                	//process response  
               		 $.each(data, function(val){  
                 	suggestions.push(val);  
             	});  
             	add(suggestions); 

				}
			});
		}
	},
	focus : function(event, ui) {
        $(this).val(ui.item.value);
        return false;
    },
    select : function(event, ui) {
    	writeAndMove(event,ui.item.value,true);
    
    }
	});

	$('#tagField').bind("keyup keypress", function(e) {
	  var code = e.keyCode || e.which; 
	  if (code  == 13) {               
	     writeAndMove(e,$('#tagField').val(),true);
	    return false;
	  }
	});

	$('#tagField').focusout(function(e) {          
	     writeAndMove(e,$('#tagField').val(),false);
	    return false;
	});

	function writeAndMove(event,val,focus){

		elements = 
		$("#tagList").append(
			$("<li/>").append($("<span/>",{class:"label label-primary"}).append(
				$("<span/>",{text:val}),
				$("<input/>",{type:"hidden",name:"tagList[]",value:val}),
				$("<span/>",{html:"&nbsp;"}),
				$("<span/>",{class:"glyphicon glyphicon-remove",style:"cursor:pointer"}
					).click(function()
							{
  								$(this).closest("li").remove();
							})
				))
			);

		$("li").remove("#addTag");

       	$("#tagList").append("<li id=\"addTag\"></li>");
       	jQuery('<input/>',{
       	type:"text", 
       	name:"tags",
       	class:"tagInput",
       	id:"tagField"
       	}).autocomplete({
		source:function(req, add) {
			var ajaxurl = "/ensc-web/ajaxTagsStartingWith/" + $("#tagField").val();
			if($("#tagField").val().length>0){
			 	$.ajax({
			        url: ajaxurl,
			        type:"GET",
			        dataType:'json',
			        async: true,
	            	cache: true,
			        success: function( data ) {
			        	var suggestions = [];  
	                	//process response  
	               		 $.each(data, function(val){  
	                 	suggestions.push(val);  
	             	});  
	             	add(suggestions); 

					}
				});
			}
		},
		focus : function(event, ui) {
	        $(this).val(ui.item.value);
	        return false;
	    },
	    select : function(event, ui) {
	    	writeAndMove(event,ui.item.value);
	    }
	}).on("keypress",function(event){
		if(event.which ==13){// 13 = 'enter'
			writeAndMove(event,$(this).val(),true);
		}
	}).focusout(function(e) {  
		if($(this).val().length>0)        
	     	writeAndMove(e,$(this).val());
	    return false;
	}).appendTo("#addTag");
	if(focus){
		$("#tagField").focus();
	}
	}
	
