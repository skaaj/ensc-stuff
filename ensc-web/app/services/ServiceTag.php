<?php


namespace Services;
class ServiceTag{

	public static function findTagsStartingWith($start, \Data\DataProvider $dataProvider){
		$propertiesTag = new \Entities\Tag($start, $dataProvider);
		$matchingTags = \Entities\Tag::findByProperties($dataProvider, $propertiesTag, TRUE, FALSE, FALSE);
		return $matchingTags;
	}

	public static function createNewTagsIfNotExist(array $tags, \Data\DataProvider $dataProvider){
		foreach($tags as $tag){
			$witnessTag=\Entities\Tag::findById($dataProvider, $tag);
			if($witnessTag==null){
				$newTag = new \Entities\Tag($tag, $dataProvider);
				$newTag->create();
			}
		}
	}

	public static function deleteReferencesBetweenTagsAndPost(\Data\DataProvider $dataProvider, \Entities\Post $post){
		$tags = \Entities\ReferenceTagPost::findTagsFromPost($post, $dataProvider);
		\Entities\ReferenceTagPost::deleteMultipleTagReferencesFromAUniquePost($post, $tags, $dataProvider);
	}


	public static function listAllTags(\Data\DataProvider $dataProvider){
		return \Entities\Tag::findAllWithOrder($dataProvider, array("name"=>\Data\OrderPolicy::ASCENDING));
	}






}
?>