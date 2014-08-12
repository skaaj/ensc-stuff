<?php

namespace Entities;
class VoteUserPost{
		private static $tableName="vote_user_post";



		public static function addVote(\Data\DataProvider $dataProvider, User $user, Post $post){
			$elements=array("userId"=>$user->getId(), "postId"=>$post->getId());
			return $dataProvider->insert(self::$tableName, $elements);
		}

		public static function userAlredyVotedToPost(User $user, Post $post, \Data\DataProvider $dataProvider){
			$columns=array("postId, userId");
			$tables=array(self::$tableName);
			$properties=array("userId"=>$user->getId(), "postId"=>$post->getId());
			$savesFound = $dataProvider->selectFieldsFromTableWithProperties($tables, $columns, $properties, TRUE, TRUE, TRUE);
			return count($savesFound)>0;
		}

		public static function deleteVotesFromUser(\Data\DataProvider $dataProvider, User $user){
			$values=array("userId"=>$user->getId());
			return $dataProvider->delete(self::$tableName, $values);
		}

		public static function deleteVotesFromPost(\Data\DataProvider $dataProvider, Post $post){
			$values=array("postId"=>$post->getId());
			return $dataProvider->delete(self::$tableName, $values);
		}

	}
?>