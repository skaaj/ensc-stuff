<?php

namespace Services;

class ServicePost{



	public static function findPostsFromQuestion(\Data\DataProvider $dataProvider, $idPostQuestion){

		$questionPost = \Entities\Post::findById($dataProvider, $idPostQuestion);
		$propertiesPost = new \Entities\Post(null, null, null, null, null, null, null, $questionPost, $dataProvider);
		$order = array("date"=>\Data\OrderPolicy::ASCENDING);
		$posts=array($questionPost);

		$responsePosts = \Entities\Post::findByPropertiesAndOrder( $dataProvider, $propertiesPost, true, false, $order, true);
		$allPosts = array_merge($posts, $responsePosts);
		return $allPosts;
	}

	public static function postResponse(\Data\DataProvider $dataProvider, $questionId, $message, \ValueObjects\UserVO $userVO){
		$date=date("Y-m-d H:i:s");
		$message = str_replace("'", "\'", $message);
		$message = str_replace("\"", "\\\"", $message);
		$userBO = \Entities\User::findById($dataProvider, $userVO->getId());
		$postType=\Entities\PostType::findByProperties( $dataProvider, new \Entities\PostType(false, "response", $date), false, false, false)[0];
		$questionPost = \Entities\Post::findById($dataProvider, $questionId);
		$newPost = new \Entities\Post(null, $date, $message, $userBO, 'false', $postType, 0, $questionPost, $dataProvider);
		$newPost->create();
	}


	public static function addVoteToPost(\Data\DataProvider $dataProvider, $idPost, $userVO){
		$post = \Entities\Post::findById($dataProvider, $idPost);
		$post->ignorePreviousChanges();
		$post->setNbvotes($post->getNbvotes()+1);
		$post->update();

		$author = $post->getAuthor();
		$author->ignorePreviousChanges();
		$author->setNbvotes($author->getNbvotes()+1);
		$author->update();
		$voter = \Entities\User::findById($dataProvider, $userVO->getId());
		\Entities\VoteUserPost::addVote($dataProvider, $voter, $post);
		return $post->getNbvotes();
	}
	public static function deleteVoteToPost(\Data\DataProvider $dataProvider, $idPost, $userVO){
		$post = \Entities\Post::findById($dataProvider, $idPost);
		$post->ignorePreviousChanges();
		$post->setNbvotes($post->getNbvotes()-1);
		$post->update();

		$author = $post->getAuthor();
		$author->ignorePreviousChanges();
		$author->setNbvotes($author->getNbvotes()-1);
		$author->update();
		$voter = \Entities\User::findById($dataProvider, $userVO->getId());
		\Entities\VoteUserPost::addVote($dataProvider, $voter, $post);
		return $post->getNbvotes();
	}

	public static function researchQuestionAndPostContainingText(\Data\DataProvider $dataProvider, $text){
		$text = str_replace("\\", "\\\\", $text);
		$text = str_replace("'", "\'", $text);
		$text = str_replace("\"", "\\\"", $text);
		$tablesPost = array("post");
		$matchingFieldsPost = array("message");
		$columnsPost=array("id", "parent");
		$posts = $dataProvider->research($tablesPost, $matchingFieldsPost, $columnsPost, $text);
		$tablesQuestion = array("question");
		$matchingFieldsQuestion = array("title");
		$columnsQuestion=array("idPost");
		$questions = $dataProvider->research($tablesQuestion, $matchingFieldsQuestion, $columnsQuestion, $text);
		$knownQuestions=null;
		foreach($questions as $q){
			$knownQuestions[]=$q->idPost;
		}

		foreach($posts as $p){
			$idQuestion = ($p->parent == null)?$p->id : $p->parent;
			if($knownQuestions==null || ($knownQuestions!=null && !in_array($idQuestion, $knownQuestions))){
				$knownQuestions[]=$idQuestion;
			}
		}
		$questions=null;
		if($knownQuestions!=null){
			foreach($knownQuestions as $idQuestion){
				$question = \Entities\Question::findById($dataProvider, $idQuestion);
				$questions[]=$question;
			}
		}
		return $questions;

	}

	public static function loadPostsFromQuestion(\Data\DataProvider $dataProvider, array $questions){
		$posts = null;
		foreach($questions as $question){
			$posts[] = \Entities\Post::findById($dataProvider, $question->getPost()->getId());
		}
		return $posts;
	}


	public static function userAlredyVotedToPost(\ValueObjects\UserVO $userVO, \Entities\Post $post, \Data\DataProvider $dataProvider){
		$user = \Entities\User::findById($dataProvider, $userVO->getId());
		return \Entities\VoteUserPost::userAlredyVotedToPost($user, $post, $dataProvider);
	}

	public static function getCorrespondingTableBetweenVotesUserPosts(\Data\DataProvider $dataProvider, \ValueObjects\UserVO $user, array $posts ){
		$correspondance = null;
		foreach($posts as $post){
			if(self::userAlredyVotedToPost($user, $post, $dataProvider)){
				$correspondance[''.$post->getId().'']=true;
			}
			else{
				$correspondance[''.$post->getId().'']=false;
			}
		}

		return $correspondance;
	}


	public static function findPostsByUser(\Data\DataProvider $dataProvider, \Entities\User $user){
		$postProperties = new \Entities\Post(null, null, null, $user, null, null, null, null, $dataProvider);
		$foundPosts = \Entities\Post::findByPRoperties($dataProvider, $postProperties, true, false, false);
		return $foundPosts;
	}
	public static function findPostsByUserId(\Data\DataProvider $dataProvider, $userId){
		$author=\Entities\User::findById($dataProvider, $userId);
		$postProperties = new \Entities\Post(null, null, null, $author, null, null, null, null, $dataProvider);
		$foundPosts = \Entities\Post::findByPRoperties($dataProvider, $postProperties, true, false, false);
		return $foundPosts;
	}
		public static function findPostsByUserIdMostRecentFirst(\Data\DataProvider $dataProvider, $userId){
		$author=\Entities\User::findById($dataProvider, $userId);
		$postProperties = new \Entities\Post(null, null, null, $author, null, null, null, null, $dataProvider);
		$order=array("date"=>\Data\OrderPolicy::DESCENDING);
		$foundPosts = \Entities\Post::findByPRopertiesAndOrder($dataProvider, $postProperties, true, false, $order, false);
		return $foundPosts;
	}

	public static function markPostAsSolution(\Data\DataProvider $dataProvider, $idPost){
		$post = \Entities\Post::findById($dataProvider, $idPost);
		$post ->ignorePreviousChanges();
		$post -> setSolution(true);
		$question = \Entities\Question::findById($dataProvider, $post->getParent()->getId());
		$question->setSolved(true);

		$updatePost = $post->update();
		$updateResult = $question->update();
		return ($updatePost && $updateResult);
	}


	public static function deletePost(\Data\DataProvider $dataProvider, \Entities\Post $post){
			\Entities\VoteUserPost::deleteVotesFromPost($dataProvider, $post);
			ServiceReport::deleteReportFromPost($dataProvider, $post);
			$post->delete();
	}

	public static function deletePostFromReport(\Data\DataProvider $dataProvider, $idReport){
		$report = \Entities\Report::findById($dataProvider, $idReport);
		$post = $report->getPost();
		if($post->getParent()==null){
			ServiceQuestion::deleteQuestion($dataProvider, $post->getId());
		}
		else{
			self::deletePost($dataProvider, $post);
		}
	}

}


?>