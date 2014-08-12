<?php

namespace Services;
class ServiceUser{

	public static function loadTopMembers(\Data\DataProvider $dataProvider){
		$dataProvider->setMaxSelectRows(10);
		$order = array("nbvotes"=> \Data\OrderPolicy::DESCENDING);
		$topMembers = \Entities\User::findAllWithOrder($dataProvider, $order);
		$dataProvider->setMaxSelectRows(100);
		return self::removeUnhautorizedUsersFromList($topMembers);
	}

	public static function removeUnhautorizedUsersFromList(array $userList){
		$i=0;
		foreach ($userList as $user) {	
			if(!$user->isAuthorized()){
				unset($userList[$i]);
			}
			$i++;
		}
		return $userList;
	}


	public static function updateAccount(\Data\DataProvider $dataProvider, \ValueObjects\UserVO $originalUser, \ValueObjects\UserVO $newInfos){
		$user = \Entities\User::findById($dataProvider, $originalUser->getId());
		$user->ignorePreviousChanges();
		$user ->setFirstName($newInfos->getFirstName());
		$user ->setLastName($newInfos->getLastName());
		$user ->setEmail($newInfos->getEmail());
		$user->update();
		return $user->toValueObject();
	}

	public static function deleteAccount(\Data\DataProvider $dataProvider, \ValueObjects\UserVO $userToDeleteVO){
		$userVerif = new \Entities\User(null, null, null, null, null, null, null, null, null, $dataProvider);
		$userVerif->setPassword($userToDeleteVO->getPassword());
		$userVerif->setId($userToDeleteVO->getId());
		$existingUsers = \Entities\User::findByProperties($dataProvider, $userVerif, true, false);
		if(count($existingUsers)>0){
			$userToDelete = \Entities\User::findById($dataProvider, $userToDeleteVO->getId());
			$usersPosts = ServicePost::findPostsByUser($dataProvider, $userToDelete);
			$roleDeleted = new \Entities\Role(null, "deleted", $dataProvider);
			
			$roleToFound= \Entities\Role::findByProperties($dataProvider, $roleDeleted, true, false, false);
			if(count($roleToFound)>0){
				$roleDeleted = $roleToFound[0];
			}
			else{
				$roleDeleted = new Role(null, "deleted", $dataProvider);
				$roleDeleted->create();
				$roleDeleted = \Entities\Role::findByProperties($dataProvider, $roleDeleted, true, false, false)[0];
			}
			$userDeleted = new \Entities\User(null, null, null, null, null, null, $roleDeleted, null, null, $dataProvider);
			$userDeletedFound = \Entities\User::findByProperties($dataProvider, $userDeleted, true, false);
			if(count($userDeletedFound)>0){
				$userDeleted=$userDeletedFound[0];
			}
			else{
				$userDeleted->create();
				\Entities\User::findByProperties($dataProvider, $userDeleted, true, false)[0];
			}
			$updatedDataBase = true;
			foreach($usersPosts as $post){
				$post->ignorePreviousChanges();
				$post->setAuthor($userDeleted);
				$update = $post->update();
				$updatedDataBase = ($update && $updatedDataBase);
			}
			\Entities\VoteUserPost::deleteVotesFromUser($dataProvider, $userToDelete);
			\Entities\Save::deleteAllSaveFromUser($dataProvider, $userToDelete);
			$userToDelete->delete();
		}
	}

	public static function listAllUsers(\Data\DataProvider $dataProvider){
		return \Entities\User::findAll($dataProvider);
	}
	public static function listAllRoles(\Data\DataProvider $dataProvider){
		return \Entities\Role::findAll($dataProvider);
	}

	public static function updateRoleUser(\Data\DataProvider $dataProvider, $idUser, $idRole){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$role = \Entities\Role::findById($dataProvider, $idRole);
		$user->ignorePreviousChanges();
		$user->setRole($role);
		$user->update();
	}

	public static function authorizeUser(\Data\DataProvider $dataProvider, $idUser){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$user->ignorePreviousChanges();
		$user->setAuthorized(true);
		$user->update();
	}
	public static function unauthorizeUser(\Data\DataProvider $dataProvider, $idUser){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$user->ignorePreviousChanges();
		$user->setAuthorized(false);
		$user->update();
	}

	public static function findSavedPostsByUser(\Data\DataProvider $dataProvider, $idUser){
		$user = \Entities\User::findById($dataProvider, $idUser);
		return \Entities\Save::getSavedPostsByUser($user, $dataProvider);
	}

	public static function findSavedQuestionsByUser(\Data\DataProvider $dataProvider, $idUser){
		$posts = self::findSavedPostsByUser($dataProvider, $idUser);
		$questions=null;
		foreach($posts as $post){
			$questions[]=\Entities\Question::findById($dataProvider, $post->getId());
		}
		return $questions;
	}

	public static function deleteSave(\Data\DataProvider $dataProvider, $idUser, $idPost){
		$user = \Entities\User::findById($dataProvider, $idUser);
		$post = \Entities\Post::findById($dataProvider, $idPost);
		\Entities\Save::deleteSave($user, $post, $dataProvider);
	}

	public static function findUserById(\Data\DataProvider $dataProvider, $idUser){
		return \Entities\User::findById($dataProvider, $idUser);
	}


}
?>