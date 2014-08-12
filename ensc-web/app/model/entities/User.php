<?php
namespace Entities;
require_once ("iEntitie.php");
require_once ("Role.php");
require_once ("app/model/valueObjects/UserVO.php");

class User implements iEntitie {
	private $dataProvider;
	private static $tableName = "user";
	private $id;
	private $login;
	private $password;
	private $firstname;
	private $lastname;
	private $role;
	private $authorized;
	private $nbvotes;
	private $email;
	private $changedValues;
	function __construct($id = null, $email = null, $login = null, $password = null, $firstname = null, $lastname = null, $role = null, $authorized=null, $nbvotes=null, $dataProvider) {
		$this -> dataProvider = new \Data\DataProvider();
		$this->changedValues = array();
		if ($id != null) {
			 $this -> id = $id;
		}
		if ($email!= null) {
			 $this -> email = $email;
		}
		if ($login != null) {
			 $this -> login = $login;
		}
		if ($password != null) {
			 $this -> password = $password;
		}
		if ($firstname != null) {
			 $this -> firstname = $firstname;
		}
		if ($lastname != null) {
			 $this -> lastname = $lastname;
		}
		if ($role != null) {
			 $this -> role = $role;
		}
		if ($authorized != null){
			$this -> authorized = $authorized;
		}
		if ($nbvotes != null){
			$this -> nbvotes = $nbvotes;
		}
		else{
			$this -> nbvotes = 0;
		}
	}

	public function getLogin() {
		return $this -> login;
	}
	public function setLogin($l){
		$this->login = $l;
		$this->changedValues["login"]=$l;
	}
	public function getId() {
		return $this -> id;
	}
	public function setId($id){
		$this->id=$id;
	}
	public function getFirstName(){
		return $this->firstname;
	}
	public function setFirstName($fn){
		$this->firstname=$fn;
		$this->changedValues["firstname"]=$fn;
	}
	public function getLastName(){
		return $this->lastname;
	}
	public function setLastName($ln){
		$this->lastname = $ln;
		$this->changedValues["lastname"]=$ln;
	}
	public function setRole(Role $role){
		$this->role=$role;
		$this->changedValues["role"]=$role->getId();

	}
	public function getRole(){
		return $this->role;
	}
	public function isAuthorized(){
		return $this->authorized;
	}
	public function setAuthorized($a){
		$this->authorized=$a;
		$this->changedValues["authorized"]=$a;
	}
	public function getPassword(){
		return $this->password;
	}
	public function setPassword($p){
		$this->password=$p;
		$this->changedValues["password"]=$p;
	}

	public function getNbvotes(){
		return $this->nbvotes;
	}
	public function setNbvotes($n){
		$this->nbvotes = $n;
		$this->changedValues["nbvotes"]=$n;
	}
	
	public function getEmail(){
		return $this->email;
	}
	
	public function setEmail($e){
		$this->email=$e;
		$this->changedValues["email"]=$e;
	}
	public function create() {
		$elements = array(
	 "login"=>$this->login, 
	 "password"=>$this->password, 
	 "firstname"=>$this->firstname, 
	 "lastname"=>$this->lastname, 
	 "role"=>$this->role->getId(), 
	 "authorized"=>$this->authorized, 
	 "nbvotes"=>$this->nbvotes, 
	 "email"=>$this->email
		);
		return $this -> dataProvider -> insert(self::$tableName, $elements);
	}

	public function delete() {
		$values = array("id" => $this -> id);
		$this -> dataProvider -> delete(self::$tableName, $values);
	}

	public function update() {
		$this->dataProvider->update(self::$tableName, $this->changedValues, array("id"=>$this->id));
		$this->changedValues = array();
	}

	public static function findAll(\Data\DataProvider $dataProvider) {
		$datas = $dataProvider -> selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach ($datas as $tuple) {
			$user = new User($tuple->id, $tuple->email, $tuple->login, $tuple->password, $tuple->firstname, $tuple->lastname, null, $tuple->authorized, $tuple->nbvotes, $dataProvider);
			$roleDatas = $dataProvider->selectFieldsFromTableWithProperties(array("role"), array("id", "name"), array("id"=>$tuple->role), false, TRUE, TRUE, TRUE);
			$role= new Role($roleDatas[0]->id, $roleDatas[0]->name, $dataProvider);
			$user->setRole($role);
			$allFound[] =$user;
		}
		return $allFound;
	}

	public static function findAllWithOrder(\Data\DataProvider $dataProvider, array $order){
		$datas = $dataProvider -> selectAllFromTableWithOrder(self::$tableName, $order);
		$count = count($datas);
		$allFound = array();
		foreach ($datas as $tuple) {
			$user = new User($tuple->id, $tuple->email, $tuple->login, $tuple->password, $tuple->firstname, $tuple->lastname, null, $tuple->authorized, $tuple->nbvotes, $dataProvider);
			$roleDatas = $dataProvider->selectFieldsFromTableWithProperties(array("role"), array("id", "name"), array("id"=>$tuple->role), false, TRUE, TRUE, TRUE);
			$role= new Role($roleDatas[0]->id, $roleDatas[0]->name, $dataProvider);
			$user->setRole($role);
			$allFound[] =$user;
		}
		return $allFound;
	}

	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false) {
		$props = array();
		$cols = array("id", "email", "login", "password", "firstname", "lastname", "role", "authorized", "nbvotes");
		$props["id"] = $ref -> getId();
		$props["login"]=$ref->getLogin();
		$props["email"]=$ref->getEmail();
	 	$props["password"]=$ref->getPassword();
	 	$props["firstname"]=$ref->getFirstname();
	 	$props["lastname"]=$ref->getLastname();
	 	if($ref->getRole()!=null){
	 		$props["role"]=$ref->getRole()->getId();
	 	}
	 	else{
	 		$props["role"]=null;
	 	}
	 	$props["authorized"]=$ref->isAuthorized();
	 	$props["nbvotes"]=$ref->getNbvotes();

		$matchs = $dataProvider -> selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects = array();
		foreach ($matchs as $tuple) {
			$user = new User($tuple->id, $tuple->email, $tuple->login, $tuple->password, $tuple->firstname, $tuple->lastname, null, $tuple->authorized, $tuple->nbvotes, $dataProvider);
			$role= Role::findById($dataProvider, $tuple->role);
			$user->setRole($role);
			$matchingObjects[] = $user;
		}

		return $matchingObjects;
	}

	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("id", "email", "login", "password", "firstname", "lastname", "role", "authorized", "nbvotes");
		$tables = array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols, array("id"=>$id), TRUE, TRUE, TRUE, TRUE);
		$result = null;
		$user = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$role = \Entities\Role::findById($dataProvider, $result->role);
			$user = new User($result->id, $result->email, $result->login, $result->password, $result->firstname, $result->lastname, $role, $result->authorized, $result->nbvotes, $dataProvider);
		}
		return $user;
	}

	public function toValueObject(){
		return new \ValueObjects\UserVO($this->getId(), 
			$this->email, 
			$this->login, 
			$this->password, 
			$this->firstname, 
			$this->lastname, 
			$this->role->toValueObject(), 
			$this->authorized, 
			$this->nbvotes);
			
	}
	public function ignorePreviousChanges(){
		$this->changedValues=array();
	}
}
?>
