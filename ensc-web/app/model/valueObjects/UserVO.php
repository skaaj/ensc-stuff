<?php
namespace ValueObjects;

require_once ("RoleVO.php");

class UserVO implements \Serializable{
	private $id;
	private $login;
	private $password;
	private $firstname;
	private $lastname;
	private $role;
	private $authorized;
	private $nbvotes;
	private $email;

	function __construct($id = null, $email = null, $login = null, $password = null, $firstname = null, $lastname = null, $role = null, $authorized=null, $nbvotes=null) {

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
	}
	public function getLastName(){
		return $this->lastname;
	}
	public function setLastName($ln){
		$this->lastname = $ln;
	}
	public function setRole(Role $role){
		$this->role=$role;
	}
	public function getRole(){
		return $this->role;
	}
	public function isAuthorized(){
		return $this->authorized;
	}
	public function setAuhorized($a){
		$this->authorized=$a;
	}
	public function getPassword(){
		return $this->password;
	}
	public function setPassword($password){
		$this->password=$password;
	}
	public function getNbvotes(){
		return $this->nbvotes;
	}
	public function setNbvotes($n){
		$this->nbvotes = $n;
	}
	
	public function getEmail(){
		return $this->email;
	}
	
	public function setEmail($e){
		$this->email=$e;
	}
	public function serialize() {
 	return serialize(
            array(
            	"id" => $this->id,
               	"login"=>$this->login,
				"password"=>$this->password,
	 			"firstname"=>$this->firstname,
	 			"lastname"=>$this->lastname,
	 			"role"=>$this->role->serialize(),
	 			"authorized"=>$this->authorized,
	 			"nbvotes"=>$this->nbvotes,
	 			"email"=>$this->email
            )
        );
	}
	public function unserialize($data) {
		$data = unserialize($data);
		$this->id=$data["id"];
		$this->login = $data["login"];
		$this->password= $data["password"];
	 	$this->firstname= $data["firstname"];
	 	$this->lastname= $data["lastname"];
	 	$unsrole = new RoleVO();
	 	$unsrole->unserialize($data["role"]);
	 	$this->role=$unsrole;
	 	$this->authorized= $data["authorized"];
	 	$this->nbvotes= $data["nbvotes"];
	 	$this->email= $data["email"];
	}
}
?>
