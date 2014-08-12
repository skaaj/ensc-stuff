<?php

namespace Services;
class ServiceReport{

	public static function reportPost(\Data\DataProvider $dataProvider, $postId, $comment){

		$post = \Entities\Post::findById($dataProvider, $postId);
		echo $post->getId();	
		echo $post->getMessage();
		$propertiesReport = new \Entities\Report(null, null, $post, $dataProvider);
		$foundReports = \Entities\Report::findByProperties($dataProvider, $propertiesReport, true, false, false);

		$report = null;
		if(count($foundReports)>0){
			$report = $foundReports[0];
			$report->setComments($report->getComments()." --- ".$comment);
			$report->update();
		}
		else{
			$report = new \Entities\Report(null, $comment, $post, $dataProvider);
			$report->create();
		}
	}

	public static function deleteReportFromPost(\Data\DataProvider $dataProvider, \Entities\Post $post){
		$reportRef = new \Entities\Report(null, null, $post, $dataProvider);
		$reportsToDelete = \Entities\Report::findByProperties($dataProvider, $reportRef, true, false, true);
		foreach ($reportsToDelete as $report) {
			$report->delete();
		}
	}
	public static function deleteReportById(\Data\DataProvider $dataProvider, $id){
		$report = \Entities\Report::findById($dataProvider, $id);
		$report->delete();
	}

	public static function listAllReports(\Data\DataProvider $dataProvider){
		return \Entities\Report::findAll($dataProvider);
	}
}
?>