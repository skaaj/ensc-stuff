<?php

namespace Entities;
class Save{
	
	private static $tableName="save";
		
	public static function addSave(User $user, Post $post, \Data\DataProvider $dataProvider){
		$elements=array("userId"=>$user->getId(), "postId"=>$post->getId());
		return $dataProvider->insert(self::$tableName, $elements);
	}
	
	public static function deleteSave(User $user, Post $post, \Data\DataProvider $dataProvider){
		$elements=array("userId"=>$user->getId(), "postId"=>$post->getId());
		$dataProvider->delete(self::$tableName, $elements);
	}
	public static function deleteAllSaveFromUser(\Data\DataProvider $dataProvider, User $user){
		$elements=array("userId"=>$user->getId());
		$dataProvider->delete(self::$tableName, $elements);
	}
	public static function deleteAllSaveFromPost(\Data\DataProvider $dataProvider, Post $post){
		$elements=array("postId"=>$post->getId());
		$dataProvider->delete(self::$tableName, $elements);
	}
	
	public static function getSavedPostsByUser(User $user, \Data\DataProvider $dataProvider){
		$columns=array("postId");
		$tables=array(self::$tableName);
		$properties=array("save.userId"=>$user->getId());
		$savedPostsIds = $dataProvider->selectFieldsFromTableWithProperties($tables, $columns, $properties, FALSE, FALSE, TRUE);	
		$savedPosts = array();
		foreach ($savedPostsIds as $currentPost){
			$post = Post::findById($dataProvider, $currentPost->postId);
			$savedPosts[] = $post;
		}
		return $savedPosts;
	}
	public static function saveAlreadyExists(User $user, Post $post, \Data\DataProvider $dataProvider){
		$columns=array("postId, userId");
		$tables=array(self::$tableName);
		$properties=array("save.userId"=>$user->getId(), "save.postId"=>$post->getId());
		$savesFound = $dataProvider->selectFieldsFromTableWithProperties($tables, $columns, $properties, TRUE, TRUE, TRUE);
		return count($savesFound)>0;
	}
	
}


?>
