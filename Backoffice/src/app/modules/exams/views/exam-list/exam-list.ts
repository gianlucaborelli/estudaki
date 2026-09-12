import { Component, inject, signal, ViewChild } from '@angular/core';
import { MATERIAL_MODULES } from '../../../../shared/imports/material.imports';
import { ExamService } from '../../services/exam.service';
import { PublicNotice } from '../../models/publicNotice';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  imports: [
    ...MATERIAL_MODULES
  ],
  selector: 'app-exam-list',
  styleUrl: './exam-list.css',
  templateUrl: './exam-list.html',
})
export class ExamList {
  private examService = inject(ExamService);
  publicNotices = new MatTableDataSource<PublicNotice>([]);
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns: string[] =
    [
      'ExaminerOrganization',
      'ContractingOrganization',
      'Year',
      'Number',
      'ExamCategory',
      'IsReviewed',
      'IsPublished',
      'actions',
    ];

  ngAfterViewInit(): void {
    this.loadExams();

    this.paginator.page.subscribe(() => {
      this.loadExams();
    });

    this.sort.sortChange.subscribe(() => {
      this.paginator.firstPage();
      this.loadExams();
    });
  }

  private loadExams(): void {
    const pageNumber = this.paginator.pageIndex;
    const pageSize = this.paginator.pageSize;

    const sortColumn = this.sort.active || undefined;
    const sortDirection = this.sort.direction || undefined;

    this.examService
      .getExamList(
        pageNumber,
        pageSize,
        sortColumn,
        sortDirection
      )
      .subscribe((data) => {
        this.publicNotices.data = data.items;
        this.paginator.length = data.totalItems;
      });
  }

  editPublicNotice(publicNotice: PublicNotice) {

  }

  deletePublicNotice(publicNotice: PublicNotice) {

  }
}
