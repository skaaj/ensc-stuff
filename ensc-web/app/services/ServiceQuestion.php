<?php

namespace Services;

class ServiceQuestion {

	public static function createQuestion(\Data\DataProvider $dataProvider, $title, $message, $user, array $tags=null){
		$unsuser = new \ValueObjects\UserVO();

		$title = str_replace("\\", "\\\\", $title);
		$title = str_replace("'", "\'", $title);
		$title = str_replace("\"", "\\\"", $title);

		$message = str_replace("\\", "\\\\", $message);
		$message = str_replace("'", "\'", $message);
		$message = str_replace("\"", "\\\"", $message);

		$unsuser->unserialize($user);
		$date=date("Y-m-d H:i:s");
		$newPost = new \Entities\Post(null, $date, $message, $unsuser, 'false', null, '0', null, $dataProvider);
		$postType=\Entities\PostType::findByProperties($dataProvider, new \Entities\PostType(false, "question", $date), false, false, false)[0];
		$newPost->setType($postType);
		$newPost->create();
		var_dump($tags);
		echo $newPost->getId();
		if($newPost->getId()!=null){
			$question = new \Entities\Question(\Entities\Post::findById($dataProvider, $newPost->getId()), $title, false, $dataProvider);
			$question->create();
			if(count($tags)>0){
				ServiceTag::createNewTagsIfNotExist($tags, $dataProvider);
				foreach($tags as $tagname){
					$tagObjects[]=\Entities\Tag::findById($dataProvider, $tagname);
				}
				\Entities\ReferenceTagPost::createMultipleTagReferencesToAUniquePost($newPost, $tagObjects, $dataProvider);
			}
		}
		return $newPost->getId();
	}

	public static function saveQuestion(\Data\DataProvider $dataProvider, $idPost, $idUser){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$post = \Entities\Post::findById($dataProvider, $idPost);
		$result = false;
		if($user!=null && $post!=null){
			if(\Entities\Save::addSave($user, $post, $dataProvider))
				{$result = true;}
		}
		return $result;
	}

	public static function saveAlreadyExists(\Data\DataProvider $dataProvider, $idPost, $idUser){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$post = \Entities\Post::findById($dataProvider, $idPost);
		return \Entities\Save::saveAlreadyExists( $user, $post, $dataProvider);
	}

	public static function getTagsFromQuestion(\Data\DataProvider $dataProvider, \Entities\Question $question){
		$post = \Entities\Post::findById($dataProvider, $question->getPost()->getId());
		return \Entities\ReferenceTagPost::findTagsFromPost($post, $dataProvider);
	}
	public static function findQuestionByIdPost(\Data\DataProvider $dataProvider, $idPost){
		return \Entities\Question::findById($dataProvider, $idPost);
	}

	public static function deleteQuestion(\Data\DataProvider $dataProvider, $idPost){
		$question = \Entities\Question::findById($dataProvider, $idPost);
		$post = \Entities\Post::findById($dataProvider, $idPost);

		ServiceTag::deleteReferencesBetweenTagsAndPost($dataProvider, $post);

		ServiceReport::deleteReportFromPost($dataProvider, $post);

		\Entities\Save::deleteAllSaveFromPost($dataProvider, $post);
		$responseProps = new \Entities\Post(null, null, null,  null, null, null, null, $post, $dataProvider);
		$responses = \Entities\Post::findByProperties($dataProvider, $responseProps, true, false, true);
		foreach($responses as $response){
			ServicePost::deletePost($dataProvider, $response);
		}
		$question->delete();
		ServicePost::deletePost($dataProvider, $post);
	}


	public static function countNbResponsesFromQuestion(\Data\DataProvider $dataProvider, $idPostQuestion){
		$postQuestion = \Entities\Post::findById($dataProvider, $idPostQuestion);
		$postResponseProps = new \Entities\Post(null, null, null, null, null, null, null, $postQuestion, $dataProvider);
		return count(\Entities\Post::findByProperties($dataProvider, $postResponseProps, true, false, true));
	}
	public static function countResponsesForQuestionList(\Data\DataProvider $dataProvider, array $questions){
		$counts = null;
		foreach($questions as $question){
			$counts[]=self::countNbResponsesFromQuestion($dataProvider, $question->getPost()->getId());
		}
		return $counts;
	}

	public static function findQuestionsByUserId(\Data\DataProvider $dataProvider, $idUser){
		$author = \Entities\User::findById($dataProvider, $idUser);
		$posts = ServicePost::findPostsByUser($dataProvider, $author);
		$questions=null;
		foreach($posts as $post){
			if($post->getParent()==null){
				$questions[] = ServiceQuestion::findQuestionByIdPost($dataProvider, $post->getId());
			}
		}
		return $questions;
	}

	public static function getLastQuestionByUserId(\Data\DataProvider $dataProvider, $idUser){
		$lastsPosts = ServicePost::findPostsByUserIdMostRecentFirst($dataProvider, $idUser);
		$lastQuestion =null;
		foreach($lastsPosts as $post){
			if($post->getParent()==null){
				$idLastQuestion=$post->getId();
				$lastQuestion = \Entities\Question::findById($dataProvider, $idLastQuestion);
				break;
			}
		}
		return  $lastQuestion;
	}

	public static function findQuestionsByUserIdLastestFirst(\Data\DataProvider $dataProvider, $idUser){
		$lastsPosts = ServicePost::findPostsByUserIdMostRecentFirst($dataProvider, $idUser);
		$questions=null;
		foreach($lastsPosts as $post){
			if($post->getParent()==null){
				$questions[] = ServiceQuestion::findQuestionByIdPost($dataProvider, $post->getId());
			}
		}
		return  $questions;
	}

		public static function getTrendingQuestions(\Data\DataProvider $dataProvider){

		$dataProvider->setMaxSelectRows(100);
		//SELECT q.title,  u.id FROM question q,  user u,  post p WHERE q.idPost = p.id AND u.id = p.author ORDER BY p.nbVotes ASC,  p.date DESC;
		$tables = array("question q",  "user u", " post p ");
		$columns = array("p.id AS postID", "u.id AS authorID");
		$properties = array("p.id"=>"q.idPost", 
							"u.id"=>"p.author");
		$order=array("p.nbVotes"=>\Data\OrderPolicy::DESCENDING, "p.date"=>\Data\OrderPolicy::DESCENDING);
		$strict=TRUE;
		$fullProperties=false;

		$rawElements = $dataProvider->selectDistinctFieldsFromTableWithPropertiesAndOrder($tables,  $columns,  $properties, $strict,  $fullProperties,  $order,  FALSE, TRUE);
		$questions=null;
		foreach($rawElements as $e){
			$questions[] = \Entities\Question::findById($dataProvider, $e->postID);
		}
		$dataProvider->setMaxSelectRows(100);
		return $questions;
	}
}

?>