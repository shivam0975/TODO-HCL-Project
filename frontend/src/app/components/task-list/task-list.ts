import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './task-list.html',
  styleUrls: ['./task-list.css']
})

export class TaskListComponent implements OnInit {
  @Input() tasks: Task[] = [];
  @Output() toggleStatus = new EventEmitter<Task>();
  @Output() deleteTask = new EventEmitter<string>();

  filter: 'all' | 'Pending' | 'Completed' = 'all';
  pendingCount = 0;
  completedCount = 0;
  highPriorityCount = 0;
  filteredTasks: Task[] = [];

  ngOnInit() {
    this.updateCounts();
  }

  ngOnChanges() {
    this.updateCounts();
  }

  private updateCounts(): void {
    this.pendingCount = this.tasks.filter(t => t.status === 'Pending').length;
    this.completedCount = this.tasks.filter(t => t.status === 'Completed').length;
    this.highPriorityCount = this.tasks.filter(t => t.priority === 2).length; // High = 2
    this.updateFilteredTasks();
  }

  private updateFilteredTasks(): void {
    this.filteredTasks = this.filter === 'all' 
      ? this.tasks 
      : this.tasks.filter(t => t.status === this.filter);
  }

  setFilter(filter: 'all' | 'Pending' | 'Completed') {
    this.filter = filter;
    this.updateFilteredTasks();
  }

  trackByTaskId(_index: number, task: Task): string {
    return task.id;
  }

  isOverdue(task: Task): boolean {
    if (!task.dueDate || task.status === 'Completed') return false;
    return new Date(task.dueDate) < new Date();
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 2: return 'High';
      case 1: return 'Medium';
      case 0: return 'Low';
      default: return 'Low';
    }
  }

  onToggle(task: Task) {
    this.toggleStatus.emit(task);
  }

  onDelete(id: string) {
    this.deleteTask.emit(id);
  }
}