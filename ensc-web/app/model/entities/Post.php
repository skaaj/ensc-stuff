<?php
namespace Entities;

require_once ("iEntitie.php");

class Post implements iEntitie {
	private $dataProvider;
	private static $tableName = "post";
	private $id;
	private $date;
	private $message;
	private $author;
	private $solution;
	private $type;
	private $nbvotes;
	private $parent;
	private $changedValues;
	function __construct($id = null, $date = null, $message = null, $author = null, $solution = null, $type = null, $nbvotes = null, $parent = null, $dataProvider) {

		$this -> dataProvider = new \Data\DataProvider();
		$this->changedValues = array();
		$this -> id = $id;
		$this -> date = $date;
		$this -> message = $message;
		$this -> author = $author;
		$this -> solution = $solution;
		$this -> type = $type;
		if ($nbvotes != null){
			$this -> nbvotes = $nbvotes;
		}
		else{
			$this->nbvotes=0;
		}
			$this->parent=$parent;
	}

	public function getDate() {
		return $this -> date;
	}
	public function setDate($d){
		$this->date = $d;
		$this->changedValues["date"]=$d;
	}
	public function getId() {
		return $this -> id;
	}
	public function getMessage(){
		return $this->message;
	}
	public function setMessage($fn){
		$this->message=$fn;
		$this->changedValues["message"]=$fn;
	}
	public function getAuthor(){
		return $this->author;
	}
	public function setAuthor(User $ln){
		$this->author = $ln;
		$this->changedValues["author"]=$ln->getId();
	}
	public function setSolution($s){
		$this->solution=$s;
		$this->changedValues["solution"]=$s;
	}
	public function getSolution(){
		return $this->solution;
	}
	public function getType(){
		return $this->type;
	}
	public function setType(PostType $pt){
		$this->type=$pt;
		$this->changedValues["type"]=$pt->getId();
	}
	public function getNbvotes(){
		return $this->nbvotes;
	}
	public function setNbvotes($n){
		$this->nbvotes = $n;
		$this->changedValues["nbvotes"]=$n;
	}
	
	public function getParent(){
		return $this->parent;
	}
	public function setParent(Post $p){
		$this->parent=$p;
		$this->changedValues["parent"]=$p->getId();
	}
	
	
	public function create() {
		$elements = array(
	 	"date"=>$this->date, 
	 	"message"=>$this->message, 
		"author"=>$this->author->getId(), 
		"solution" =>$this->solution, 
		"type" =>$this->type->getId(), 
		"nbvotes" =>$this->nbvotes
		);
		if($this->parent!=null){
			$elements["parent"]=$this->parent->getId();
		}
		$this ->dataProvider -> insert(self::$tableName, $elements);
		$last = self::findByProperties($this->dataProvider, $this, TRUE, FALSE, TRUE)[0];
		$this->id = $last->getId();
	}

	public function delete() {
		$values = array("id" => $this -> id);
		$this -> dataProvider -> delete(self::$tableName, $values);
	}

	public function update() {
		$result = $this->dataProvider->update(self::$tableName, $this->changedValues, array("id"=>$this->id));
		$this->changedValues = array();
		return $result;
	}

	public static function findAll(\Data\DataProvider $dataProvider) {
		$datas = $dataProvider -> selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach ($datas as $tuple) {
			$post = new Post($tuple->id, $tuple->date, $tuple->message, false, $tuple->solution, false, $tuple->nbvotes, false, $dataProvider, TRUE);
			
			if($tuple->parent!=null){
				$parent = Post::findById($this->dataProvider, $tuple->parent);
				$post->setParent($parent);
			}
			
			$type= PostType::findById($dataProvider, $tuple->type);
			$post->setType($type);
			
			$author = User::findById($dataProvider, $tuple->author);
			$post->setAuthor($author);
			$allFound[] =$post;
		}
		return $allFound;
	}

	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false) {

		$props = array();
		$cols = array("id", "date", "message", "author", "solution", "type", "nbvotes", "parent");
		$props["id"]=$ref->getId();
		$props["date"]=$ref->getDate();
	 	$props["message"]=$ref->getMessage();
		$props["author"]=($ref->getAuthor()!=null)?$ref->getAuthor()->getId():null;
		$props["solution"] =$ref->getSolution();
		$props["type"]=($ref->getType()!=null)?$ref->getType()->getId():null;
		$props["nbvotes"] =$ref->getNbvotes();
		$props["parent"] =($ref->getParent()!=null)? $ref->getParent()->getId():null;
		
		$matchs = $dataProvider -> selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects = array();
		foreach ($matchs as $tuple) {
			$post = new Post($tuple->id, $tuple->date, $tuple->message, null, $tuple->solution, null, $tuple->nbvotes, $tuple->parent, $dataProvider);
							
			$type= PostType::findById($dataProvider, $tuple->type);
			$post->setType($type);
			
			$author = User::findById($dataProvider, $tuple->author);
			$post->setAuthor($author);
			
			if($tuple->parent!=null){
				$parent = Post::findById($dataProvider, $tuple->parent);
				$post->setParent($parent);
			}
			$matchingObjects[] = $post;
		}

		return $matchingObjects;
	}

		public static function findByPropertiesAndOrder(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, array $order, $strictTextComparizon=false) {

		$props = array();
		$cols = array("id", "date", "message", "author", "solution", "type", "nbvotes", "parent");
		$props["id"]=$ref->getId();
		$props["date"]=$ref->getDate();
	 	$props["message"]=$ref->getMessage();
		$props["author"]=($ref->getAuthor()!=null)?$ref->getAuthor()->getId():null;
		$props["solution"] =$ref->getSolution();
		$props["type"]=($ref->getType()!=null)?$ref->getType()->getId():null;
		$props["nbvotes"] =$ref->getNbvotes();
		$props["parent"] =($ref->getParent()!=null)? $ref->getParent()->getId():null;
		
		$matchs = $dataProvider -> selectFieldsFromTableWithPropertiesAndOrder(array(self::$tableName), $cols, $props, $strict, $fullProperties, $order, TRUE, $strictTextComparizon);
		$matchingObjects = array();
		foreach ($matchs as $tuple) {
			$post = new Post($tuple->id, $tuple->date, $tuple->message, null, $tuple->solution, null, $tuple->nbvotes, $tuple->parent, $dataProvider);
							
			$type= PostType::findById($dataProvider, $tuple->type);
			$post->setType($type);
			
			$author = User::findById($dataProvider, $tuple->author);
			$post->setAuthor($author);
			
			if($tuple->parent!=null){
				$parent = Post::findById($dataProvider, $tuple->parent);
				$post->setParent($parent);
			}
			$matchingObjects[] = $post;
		}

		return $matchingObjects;
	}

	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("id", "date", "message", "author", "solution", "type", "nbvotes", "parent");
		$tables = array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols , array("id"=>$id), TRUE, FALSE, TRUE, TRUE);
		$result = null;
		$post = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$post = new Post($result->id, $result->date, $result->message, null, $result->solution, null, $result->nbvotes, null, $dataProvider);
			$type= PostType::findById($dataProvider, $result->type);
			$post->setType($type);
			
			$author = User::findById($dataProvider, $result->author);
			$post->setAuthor($author);
			
			if($result->parent!=null){
				$parent = Post::findById($dataProvider, $result->parent);
				$post->setParent($parent);
			}
		}
		return $post;
			
	}

	public function ignorePreviousChanges(){
		$this->changedValues=array();
	}

	
}
?>
