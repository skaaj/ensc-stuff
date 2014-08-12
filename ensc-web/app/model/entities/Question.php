<?php



namespace Entities;

class Question implements iEntitie{
	private $dataProvider;
	private static $tableName="question";
	private $post;
	private $title;
	private $solved;
	private $changedValues;
	
	function __construct($post=false , $title=false, $solved=false, $dataProvider){
		$this->changedValues = array();
		$this->dataProvider = new \Data\DataProvider();
		
		if($post!=false){ $this->post=$post; }
		
		if($title!=false){ $this->title=$title; }
		if($solved!=false){ $this->solved=$solved; }
	}
	
	public function getTitle(){
		return $this->title;
	}
	public function getPost(){
		return $this->post;
	}
	
	public function setTitle($t){
		$this->title=$t;
	}

	public function setSolved($solved){
		$this->solved=$solved;
		$this->changedValues["solved"]=$solved;
	}
	public function getSolved(){
		return $this->solved;
	}
	
	public function create(){
		$elements = array("idPost"=>$this->post->getId(), "title"=>$this->title);
		$this->dataProvider->insert(self::$tableName, $elements);
	}
	
	public function delete(){
		$values = array("idPost"=>$this->post->getId());
		$this->dataProvider->delete(self::$tableName, $values);
	}
	
	public function update(){
		$result = $this->dataProvider->update(self::$tableName, $this->changedValues, array("idPost"=>$this->post->getId()));
		$this->changedValues = array();
		return $result;
	}
	
	public static function findAll(\Data\DataProvider $dataProvider){
		$datas = $dataProvider->selectAllFromTable(self::$tableName);
		$count = count($datas);
		$allFound = array();
		foreach($datas as $question){
			$allFound[]=new Question(Post::findById($dataProvider, $question->idPost), $question->title, $dataProvider);
		}
		return $allFound;
	}
	
	public static function findByProperties(\Data\DataProvider $dataProvider, $ref, $strict, $fullProperties, $strictTextComparizon=false){
		$props =array();
		$cols = array("idPost", "title", "solved");
		$props["idPost"] = $ref->getIdPost();
		$props["title"]=$ref->getTitle();
		$props["solved"]=$ref->getSolved();
		
		$matchs = $dataProvider->selectFieldsFromTableWithProperties(array(self::$tableName), $cols, $props, $strict, $fullProperties, TRUE, $strictTextComparizon);
		$matchingObjects=array();
		foreach($matchs as $tuple){
			$matchingObjects[]=new Question(Post::findById($dataProvider, $tuple->idPost), $tuple->title, $dataProvider);
		}
		
		return $matchingObjects;
	}
	
		
	public static function findById(\Data\DataProvider $dataProvider, $id){
		$cols = array("idPost", "title", "solved");
		$tuples = $dataProvider->selectFieldsFromTableWithProperties(array(self::$tableName), $cols, array("idPost"=>$id), TRUE, TRUE, TRUE, TRUE);
		$result = null;
		$question = null;
		if(count($tuples)>0){
			$result=$tuples[0];
			$question=new Question(Post::findById($dataProvider, $result->idPost), $result->title, $result->solved, $dataProvider);
		}
		return $question;
	}
	
	
}



?>