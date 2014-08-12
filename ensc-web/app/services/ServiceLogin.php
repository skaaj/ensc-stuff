<?php

namespace Services;
class ServiceLogin{


	private static function login(\Entities\User $user, \Data\DataProvider $dataProvider){
		$result = \Entities\User::findByProperties($dataProvider, $user, true, false, true);
		return $result;
	}

	private static function register(\Entities\User $user){
		return $user->Create();
	}



	public static function validateRegister(\Entities\User $user, $dataProvider){
		$userVerif = new \Entities\User( null, $user->getEmail(), $user->getLogin(), null, null, null, null, null, null, $dataProvider);
		//echo $userVerif->getLogin()." ; ".$userVerif->getPassword()." ; ".$userVerif->getEmail()." : ".count(\Entities\User::findByProperties($dataProvider, $userVerif, false, false));
		return ($user!=null &&
			$userVerif->getId()==null &&
			($userVerif->getLogin() != null || $userVerif->getLogin() != "")&&
			($user->getPassword() != null || $user->getPassword() != "")&&
			filter_var($userVerif->getEmail(), FILTER_VALIDATE_EMAIL) &&
			count(\Entities\User::findByProperties($dataProvider, $userVerif, false, false))==0);
	}

	public static function processRegisterUser(\Entities\User $user, $dataProvider){
		$result = false;
		if(self::validateRegister($user, $dataProvider)){
			self::register($user);
			$result = true;
		}
		return $result;
	}

	public static function processLogUser($login, $password, \Data\DataProvider $dataProvider){

		$userLog = new \Entities\User(null, null, $login, $password, null, null, null, null, null, $dataProvider);
		//var_dump($userLog);
		$users = self::login($userLog, $dataProvider);
		$result=false;
		if(count($users)!=0){
			$result=$users[0];
		}

		return $result;
	}
}
?>