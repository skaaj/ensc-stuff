<?php

namespace Entities;

class ReferenceTagPost{
	
	private static $tableName="reference_tag_post";
	
	
	public static function createMultipleTagReferencesToAUniquePost(Post $post, array $tags, \Data\DataProvider $dataProvider){
		foreach($tags as $tag){
			self::createReferences($post, $tag, $dataProvider);
		}
	}
	
	public static function createReferences(Post $post, Tag $tag, \Data\DataProvider $dataProvider){
		$elements = array("postId"=>$post->getId(), "tagName"=>$tag->getName());
		$dataProvider->insert(self::$tableName, $elements);
	}
	
	public static function deleteMultipleTagReferencesFromAUniquePost(Post $post, array $tags, \Data\DataProvider $dataProvider){
		foreach($tags as $tag){
			self::deleteReferences($post, $tag, $dataProvider);
		}
	}
	
	public static function deleteReferences(Post $post, Tag $tag, \Data\DataProvider $dataProvider){
		$elements = array("postId"=>$post->getId(), "tagName"=>$tag->getName());
		$dataProvider->delete(self::$tableName, $elements);
	}
	
	
	
	
	public static function findTagsFromPost(Post $post, \Data\DataProvider $dataProvider){
		$tables = array(self::$tableName);
		$columns= array("tagName");
		$properties = array("postId"=>$post->getId());
		$correspondingTagsNames = $dataProvider->selectFieldsFromTableWithProperties($tables, $columns, $properties, FALSE, FALSE, TRUE, TRUE);
		$correspondingTags= array();
		foreach ($correspondingTagsNames as $currentTag){
			$tag = Tag::findById($dataProvider, $currentTag->tagName);
			$correspondingTags[] = $tag;
		}
		return $correspondingTags;
	}
	
	
	public static function findPostFromTags(array $tags, \Data\DataProvider $dataProvider){
		
		$properties=array();
		foreach($tags as $t){
			$properties["tagName"]=$t->getName();
		}
		
		$columns=array("postId");
		$tables = array(self::$tableName);
		
		$correspondingPostsIds = $dataProvider->selectDistinctFieldsFromTableWithProperties($tables, $columns, $properties, FALSE, FALSE, TRUE, TRUE);
		$correspondingPosts= array();
		foreach ($correspondingPostsIds as $currentPost){
			$post = Post::findById($dataProvider, $currentPost->postId);
			$correspondingPosts[] = $post;
		}
		return $correspondingPosts;
	}
}


?>
