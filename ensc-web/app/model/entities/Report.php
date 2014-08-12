<?php

namespace Entities;

class Report implements iEntitie{
	private static $tableName="report";

	private $id;
	private $comments;
	private $post;
	private $dataProvider;
	private $changedValues; 

	public function __construct($id=null, $comments=null, Post $post=null, \Data\DataProvider $dataProvider){
		$changedValues= array();
		$this->dataProvider=$dataProvider;
		if ($id!= null) {
			 $this -> id = $id;
		}
		if ($comments != null) {
			 $this -> comments = $comments;
		}
		if ($post != null) {
			 $this -> post = $post;
		}
	}



	public function getId(){
		return $this->id;
	}
	public function getPost(){
		return $this->post;
	}
	public function getComments(){
		return $this->comments;
	}


	public function setId($id){
		$this->id=$id;
	}
	public function setPost(Post $post){
		$this->post = $post;
	}
	public function setComments($comments){
		$this->comments = $comments;
		$this->changedValues["comments"]=$comments;
	}



	public function create(){
		$elements = array("comments"=>$this->comments, 
							"postId"=>$this->post->getId());
		return $this -> dataProvider -> insert(self::$tableName, $elements);
	}
	public function delete(){
		$values = array("id" => $this -> id);
		$this -> dataProvider -> delete(self::$tableName, $values);
	}

	public function update(){
		$this->dataProvider->update(self::$tableName, $this->changedValues, array("id"=>$this->id));
		$this->changedValues = array();
	}

	public static function findAll(\Data\DataProvider $dataProvider){
		$datas = $dataProvider -> selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach ($datas as $tuple) {
			$post = Post::findById($dataProvider, $tuple->postId);
			$report = new Report($tuple->id, $tuple->comments, $post, $dataProvider);
			$allFound[]=$report;
		}
		return $allFound;
	}
	

	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false){
		$props = array();
		$cols = array("id", "comments", "postId");
		$props["id"] = $ref -> getId();
		$props["comments"]=$ref->getComments();
	 	if($ref->getPost()!=null){
	 		$props["postId"]=$ref->getPost()->getId();
	 	}
	 	else{
	 		$props["postId"]=null;
	 	}
	 	//var_dump( $props);
		$matchs = $dataProvider -> selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects = array();
		foreach ($matchs as $tuple) {
			$post = Post::findById($dataProvider, $tuple->postId);
			$report = new Report($tuple->id, $tuple->comments, $post, $dataProvider);
			$matchingObjects[] = $report;
		}

		return $matchingObjects;
	}


	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols=array("id", "comments", "postId");
		$tables = array(self::$tableName);
		$tuples = $dataProvider->selectFieldsFromTableWithProperties($tables, $cols, array("id"=>$id), TRUE, TRUE, TRUE, TRUE);
		$result = null;
		$report = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$post = Post::findById($dataProvider, $result->postId);
			$report = new Report($result->id, $result->comments, $post, $dataProvider);
		}
		return $report;
	}
}
?>