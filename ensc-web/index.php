<?php
require 'vendor/autoload.php';

use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\HttpKernel\HttpKernelInterface;
use Application\Model;

session_start();

if(!isset($_SESSION['lang'])){
	$language = $_SERVER['HTTP_ACCEPT_LANGUAGE'];
	$language = substr($language,0,2);
	$_SESSION['lang']=$language;
}

$app = new Silex\Application();
$app->register(new Silex\Provider\SerializerServiceProvider());
$app['dataProvider'] = new \Data\DataProvider();

/* TWIG */
$loader = new Twig_Loader_Filesystem(__DIR__.'/web/layout/'.$_SESSION['lang']);
$twig   = new Twig_Environment($loader);

/* DISPATCHER */

/*     
	INDEX
*/


$app->get('/', function() use($twig, $app) {
	$topMembers = \Services\ServiceUser::loadTopMembers($app['dataProvider']);
	$trendingQuestions = \Services\ServiceQuestion::getTrendingQuestions($app['dataProvider']);
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		return $twig->render('index.html', array("user" => $userVO, "topMembers"=>$topMembers, "trendingQuestions"=>$trendingQuestions));
	}else{
		return $twig->render('/index.html', array("topMembers"=>$topMembers, "trendingQuestions"=>$trendingQuestions));
	}
});

$app->get('/explore', function() use($twig, $app) {
	$trendingQuestions = \Services\ServiceQuestion::getTrendingQuestions($app['dataProvider']);
	if($trendingQuestions!=null){

		$nbResponses = \Services\ServiceQuestion::countResponsesForQuestionList($app["dataProvider"], $trendingQuestions);
			
		if(isset($_SESSION["user"])){
			$userVO = new \ValueObjects\UserVO();
			$userVO->unserialize($_SESSION["user"]);
			return $twig->render('/explore.html', array("user" => $userVO, "trendingQuestions"=>$trendingQuestions, "nbResponses"=>$nbResponses));
		}else{
			return $twig->render('/explore.html', array("trendingQuestions"=>$trendingQuestions, "nbResponses"=>$nbResponses));
		}
	}
	else{
		return $twig->render('/explore.html');
	}
});

/*
  ___ ___   _   ___  ___ _  _ 
 / __| __| /_\ | _ \/ __| || |
 \__ \ _| / _ \|   / (__| __ |
 |___/___/_/ \_\_|_\\___|_||_|
*/
$app->get('/search', function() use($twig) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		return $twig->render('/search.html', array("user" => $userVO));
	}else{
		return $twig->render('/search.html');
	}
});

$app->post("/searchtext", function(Request $request) use($twig, $app){
	$text = $request->get("search");
	return $twig->render('/search.html', array("searchtext"=>$text));
});

$app->post("/searchtextajax", function(Request $request) use($twig, $app){


	$text = $request->get("search");
	$questions = \Services\ServicePost::researchQuestionAndPostContainingText($app["dataProvider"], $text);
	$posts=null;
	$response="";

	$label_title_result="Résultats";
	$label_question="Question";
	$label_nbr_answers="Nombre de réponses";
	$label_no_answers="Aucune question correspondant à votre recherche n'a été trouvée.";
	if($_SESSION["lang"]!="fr"){
		$label_title_result="Results";
		$label_question="Question";
		$label_nbr_answers="Number of answers";
		$label_no_answers="No question corresponding to your search was found.";
	}

	if($questions!=null)
	{
		
		$posts = \Services\ServicePost::loadPostsFromQuestion($app["dataProvider"], $questions);
		$response.="<h3>".$label_title_result."</h3>";
		$response.= "<table class=\"table table-striped\"><tr><th>".$label_question."</th><th>".$label_nbr_answers."</th></tr>";	
			
		for($i=0;$i<count($questions);$i++){
			$response.= "<tr><td><a href=\"/ensc-web/view/".$questions[$i]->getPost()->getId()."\">".$questions[$i]->getTitle()."</a></td>";
			$response.= "<td>".\Services\ServiceQuestion::countNbResponsesFromQuestion($app["dataProvider"], $questions[$i]->getPost()->getId())."</td></tr>";
		};
		$response.= "</tr></table>";
	}
	else{
		$response.=  "<h4>".$label_no_answers."</h4>";
	}
	return $response;
});

/*
    _   ___ _  __
   /_\ / __| |/ /
  / _ \\__ \ ' < 
 /_/ \_\___/_|\_\
*/
$app->get('/ask', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		$questions = \Services\ServiceQuestion::findQuestionsByUserIdLastestFirst($app['dataProvider'], $userVO->getId()	);
		if($questions!=null){
			$nbResponses = \Services\ServiceQuestion::countResponsesForQuestionList($app["dataProvider"], $questions);
			return $twig->render('/ask.html', array("user" => $userVO, "lastQuestions"=>$questions, "nbResponses"=>$nbResponses));
		}else{
			return $twig->render('/ask.html', array("user" => $userVO));
		}
	}else{
		return changePage("/ensc-web/signin", $app);
	}
});


$app->post("/post_new", function(Request $request) use($twig, $app){
	$tags = $request->get("tagList");
	$newPost = \Services\ServiceQuestion::createQuestion($app["dataProvider"],  $request->get("title"),  $request->get("message"), $_SESSION["user"], $tags);
	if($newPost!=null){
		return changePage("/ensc-web/view/".$newPost, $app);
	}
	else{
		return changePage("/ensc-web/", $app);
	}
});




/*
  _    ___   ___ ___ _  _ 
 | |  / _ \ / __|_ _| \| |
 | |_| (_) | (_ || || .` |
 |____\___/ \___|___|_|\_|

*/

$app->match('/signin', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		return $twig->render('/index.html', array("user" => $userVO));
	}else{
		return $twig->render('/sign_in.html');
	}
});

$app->post("/log_user", function(Request $request) use($twig, $app){
	$user = \Services\ServiceLogin::processLogUser($request->get('login'), $request->get('password'), $app['dataProvider']);
	$userVO=null;
	if($user!=false){
		$userVO = $user->toValueObject();
		$_SESSION["user"] = $userVO->serialize();
		return $twig->render('/index.html', array("user"=>$userVO));
	}else{	
		return $twig->render('/sign_in.html',array("erreurConnexion"=>true));
	}
});

$app->get("/disconnect", function() use($twig, $app){
	session_destroy();
	session_start();
	return changePage("/ensc-web/", $app);
});

$app->post("/forgottenpassword", function(Request $request) use($twig, $app){
	$email = $request->get("email");
	\Services\ServiceForgottenPassword::sendBackupEmail($app["dataProvider"], $email);
	return changePage("/ensc-web/", $app);
});

$app->get("/passwordbackup/{email}/{key}", function($email, $key) use($twig, $app){
	if(\Services\ServiceForgottenPassword::validateBackupUrl($app["dataProvider"], $email, $key)){
		return $twig->render('/change_password.html', array("email"=>$email, "key"=>$key));
	}
	else{
		return changePage("/ensc-web/", $app);
	}
});
$app->post("/changepassword", function(Request $request) use($twig, $app){
	if(\Services\ServiceForgottenPassword::validateBackupUrl($app["dataProvider"], $request->get("email"), $request->get("key"))){
			\Services\ServiceForgottenPassword::changePassword($app["dataProvider"], $request->get("email"), $request->get("password"));
	}
	return changePage("/ensc-web/signin", $app);
});

/*
  ___ ___ ___ ___ ___ _____ ___ ___ 
 | _ \ __/ __|_ _/ __|_   _| __| _ \
 |   / _| (_ || |\__ \ | | | _||   /
 |_|_\___\___|___|___/ |_| |___|_|_\

*/

$app->get("/register", function() use($twig, $app){
	return $twig->render('/register.html');
});


$app->post("/register_user", function(Request $request) use($twig, $app){
	$role = \Entities\Role::findByProperties($app["dataProvider"], new \Entities\Role(null, "user", $app["dataProvider"]), true, false)[0];
	$newUser = new \Entities\User(null, $request->get('email'), $request->get('login'), $request->get('password'), null, null, $role, true, 0, $app['dataProvider'] ) ;
	
	$userOk = \Services\ServiceLogin::processRegisterUser($newUser, $app['dataProvider'] );
	if($userOk){
    	return changePage("/ensc-web/", $app);
	}else{
		return $twig->render('/register.html');
	}
});
$app->post("/deleteaccount", function(Request $request) use($twig, $app){
	$userId = $request->get("userId");
	$password = $request->get("password");
	if(isset($_SESSION['user'])){
		$userToDeleteVO = new \ValueObjects\UserVO();
		$userToDeleteVO->setPassword($password);
		$userToDeleteVO->setId($userId);
		\Services\ServiceUser::deleteAccount($app['dataProvider'], $userToDeleteVO);
		session_destroy();
		session_start();
	}
	return changePage("/ensc-web/", $app);

	
});


/*
   ___  _   _ ___ ___ _____ ___ ___  _  _ ___ 
  / _ \| | | | __/ __|_   _|_ _/ _ \| \| / __|
 | (_) | |_| | _|\__ \ | |  | | (_) | .` \__ \
  \__\_\\___/|___|___/ |_| |___\___/|_|\_|___/
*/



$app->get('/ajaxTagsStartingWith/{tagName}', function($tagName) use($twig, $app){
	$matchingTags = \Services\ServiceTag::findTagsStartingWith($tagName, $app["dataProvider"]);
	$jsonData="";
	$headers = array('Content-Type' => 'application/json');
	if(count($matchingTags)>0){
		foreach($matchingTags as $tag){
			$stringArray[$tag->getName()] = array("name"=>$tag->getName());
		}
		$jsonData = json_encode($stringArray);
	}
  	$response = new Response($jsonData, 200, $headers);
  	return $response;

});


$app->get("/view/{idPost}", function($idPost) use($twig, $app){

	$question = \Services\ServiceQuestion::findQuestionByIdPost($app["dataProvider"], $idPost);
	if($question!=null){
		$posts = \Services\ServicePost::findPostsFromQuestion($app["dataProvider"], $idPost);
		$tags = \Services\ServiceQuestion::getTagsFromQuestion($app["dataProvider"], $question);
		$user=null;
		$alreadySaved=true;
		$alreadyVoted=null;
		if(isset($_SESSION["user"])){
			$user = new \ValueObjects\UserVO();
			$user->unserialize($_SESSION["user"]);
			$alreadySaved = \Services\ServiceQuestion::saveAlreadyExists($app["dataProvider"], $idPost, $user->getId());
			$alreadyVoted = \Services\ServicePost::getCorrespondingTableBetweenVotesUserPosts($app["dataProvider"], $user, $posts);
		}
	return $twig->render('/question_list.html', array("question"=>$question, "posts"=>$posts, "tags"=>$tags, "user"=>$user, "alreadySaved"=>$alreadySaved, "alreadyVoted"=>$alreadyVoted));
	}
	else{
		return changePage("/ensc-web/".$questionId, $app);
	}
});


$app->post("/answer", function(Request $request) use($twig, $app){
	$message = $request->get("message");
	$questionId = $request->get("questionId");
	if(strlen($message)>0){
		
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		\Services\ServicePost::postResponse( $app["dataProvider"], $questionId, $message, $userVO);

		$question = \Entities\Question::findById($app["dataProvider"], $questionId);
	}
	return changePage("/ensc-web/view/".$questionId, $app);

});


$app->get("/addvotetopost/{idPost}", function($idPost) use($twig, $app){
	$userVO = new \ValueObjects\UserVO();
	$userVO->unserialize($_SESSION["user"]);
	$nbVotes = \Services\ServicePost::addVoteToPost($app["dataProvider"], $idPost, $userVO);
	$headers = array('Content-Type' => 'application/json');
	$stringArray["nbvotes"]=$nbVotes;
	$jsonData = json_encode($stringArray);
	$response = new Response($jsonData, 200, $headers);
  	return $response;
});
$app->get("/deletevotetopost/{idPost}", function($idPost) use($twig, $app){
	$userVO = new \ValueObjects\UserVO();
	$userVO->unserialize($_SESSION["user"]);
	$nbVotes = \Services\ServicePost::deleteVoteToPost($app["dataProvider"], $idPost, $userVO);
	$headers = array('Content-Type' => 'application/json');
	$stringArray["nbvotes"]=$nbVotes;
	$jsonData = json_encode($stringArray);
	$response = new Response($jsonData, 200, $headers);
  	return $response;
});


$app->get("/marksolution/{idPost}/{idQuestion}", function($idPost, $idQuestion) use($twig, $app){
	\Services\ServicePost::markPostAsSolution($app['dataProvider'], $idPost);
	return changePage("/ensc-web/view/".$idQuestion, $app);
});



$app->post("/report", function(Request $request) use($twig, $app){
	$comment = $request->get("comment");
	$postId = $request->get("postId");
	\Services\ServiceReport::reportPost($app["dataProvider"], $postId, $comment);
	return changePage("/ensc-web/view/".$postId, $app);
});


/*
  ___ ___  ___  ___ ___ _    ___ 
 | _ \ _ \/ _ \| __|_ _| |  | __|
 |  _/   / (_) | _| | || |__| _| 
 |_| |_|_\\___/|_| |___|____|___|
*/
$app->get('/myquestions', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		$questions = \Services\ServiceQuestion::findQuestionsByUserIdLastestFirst($app['dataProvider'], $userVO->getId()	);
		if( $questions != null){
		$nbResponses = \Services\ServiceQuestion::countResponsesForQuestionList($app["dataProvider"], $questions);
			return $twig->render('/myquestions.html', array("user" => $userVO, "questions"=>$questions, "nbResponses"=>$nbResponses));
		}else{
			return $twig->render('/myquestions.html', array("user" => $userVO));
		}
	}else{
		return changePage("/ensc-web/signin", $app);
	}
});

$app->post("/account/deletesave", function(Request $request) use($twig, $app){
	if(isset($_SESSION["user"])){
		$userId = $request->get("userId");
		$postId = $request->get("postId");
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		if($userId==$userVO->getId()){
			\Services\ServiceUser::deleteSave($app["dataProvider"], $userId, $postId);
		}
		return changePage("/ensc-web/starred", $app);
	}
	else{
		return changePage("/ensc-web/signin", $app);
	}

});


$app->get('/profile/{userId}', function($userId) use($twig, $app) {
	$userToShow = \Services\ServiceUser::findUserById($app['dataProvider'], $userId);
	$userToShow=$userToShow->toValueObject();
	$questions = \Services\ServiceQuestion::findQuestionsByUserId($app['dataProvider'], $userId);
	$nbQuestions=count($questions);
	$posts = \Services\ServicePost::findPostsByUserId($app['dataProvider'], $userId);
	$nbPosts = count($posts);
	$lastQuestion=\Services\ServiceQuestion::getLastQuestionByUserId($app['dataProvider'], $userId);
	$twigArray=array("userToShow"=>$userToShow, "nbQuestions"=>$nbQuestions, "nbPosts"=>$nbPosts, "lastQuestion"=>$lastQuestion);
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		$twigArray["user"]=$userVO;
		return $twig->render('/profile.html', $twigArray);
	}else{
		return $twig->render('/profile.html', $twigArray);
	}
});

$app->get('/parameters', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		return $twig->render('/parameters.html', array("user" => $userVO));
	}else{
		return changePage("/ensc-web/signin", $app);
	}
});

$app->post("/updateaccount", function(Request $request) use($twig, $app){
	 if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION['user']);
		$newInfos = new \ValueObjects\UserVO(null, $request->get("email"), null, null,  $request->get("firstname"), $request->get("lastname"),  null, null, null);
		$userVO = \Services\ServiceUser::updateAccount($app['dataProvider'], $userVO, $newInfos);
		$_SESSION["user"]=$userVO->serialize();
		return $twig->render('/parameters.html', array("user"=>$userVO));
	 }
	 else{
	 	return changePage("/ensc-web/signin", $app);
	 }

});


$app->get('/starred', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		$questions = \Services\ServiceUser::findSavedQuestionsByUser($app["dataProvider"], $userVO->getId());
		$responcesCounts=null;
		if($questions!=null){
			
			$responcesCounts = \Services\ServiceQuestion::countResponsesForQuestionList($app["dataProvider"], $questions);
		}
		return $twig->render('/starred.html', array("user" => $userVO, "questions"=>$questions, "responcesCounts"=>$responcesCounts));
	}else{
		return $twig->render('/starred.html');
	}
});


/*
    _   ___  __  __ ___ _  _ 
   /_\ |   \|  \/  |_ _| \| |
  / _ \| |) | |\/| || || .` |
 /_/ \_\___/|_|  |_|___|_|\_|
*/
$app->get('/admin', function() use($twig, $app) {
	if(isset($_SESSION["user"])){
		$userVO = new \ValueObjects\UserVO();
		$userVO->unserialize($_SESSION["user"]);
		if($userVO->getRole()->getName()=="administrator")
		{
			$users = \Services\ServiceUser::listAllUsers($app['dataProvider']);
			$roles = \Services\ServiceUser::listAllRoles($app['dataProvider']);
			$reports = \Services\ServiceReport::listAllReports($app['dataProvider']);
			$tags=\Services\ServiceTag::listAllTags($app['dataProvider']);
			return $twig->render('/admin.html', array("users"=>$users, "roles"=>$roles, "reports"=>$reports, "tags"=>$tags, "user"=>$userVO)); 
		}
		else{
			return changePage("/ensc-web/signin", $app); 
		}
	}
	else{
		return changePage("/ensc-web/signin", $app); 
	}
});


$app->post("/admin/updateuserrole", function(Request $request) use($twig, $app){
	$userId=$request->get("userId");
	$roleId=$request->get("roleId");
	\Services\ServiceUser::updateRoleUser($app['dataProvider'], $userId, $roleId);
	return changePage("/ensc-web/admin".$idQuestion, $app);
});

$app->post("/admin/disableuser", function(Request $request) use($twig, $app){
	$userId = $request->get("userId");
	\Services\ServiceUser::unauthorizeUser($app['dataProvider'], $userId);
	return changePage("/ensc-web/admin".$idQuestion, $app);
});
$app->post("/admin/enableuser", function(Request $request) use($twig, $app){
	$userId = $request->get("userId");
	\Services\ServiceUser::authorizeUser($app['dataProvider'], $userId);
	return changePage("/ensc-web/admin".$idQuestion, $app);
});


$app->post("/admin/deletepost", function(Request $request) use($twig, $app){
	\Services\ServicePost::deletePostFromReport($app['dataProvider'], $request->get("reportId"));
	return changePage("/ensc-web/admin".$idQuestion, $app);
});




function changePage($path, $app){
	//$subRequest = Request::create($path, 'GET');
    //return $app->handle($subRequest, HttpKernelInterface::SUB_REQUEST);
    return $app->redirect($path);
}

/* BOOTSTRAP */
$app['debug'] = false;
$app->run();


?>