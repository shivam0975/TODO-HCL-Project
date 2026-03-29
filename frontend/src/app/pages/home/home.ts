import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { TaskListComponent } from '../../components/task-list/task-list';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, TaskListComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit {
  tasks: Task[] = [];
  visibleTasks: Task[] = [];
  loading = false;
  error = '';
  selectedCategory: string | null = null;

  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      this.selectedCategory = params.get('category');
      this.applyCategoryFilter();
      this.cdr.markForCheck();
    });

    this.loadTasks();
  }

  private loadTasks(): void {
    this.loading = true;
    this.error = '';

    this.taskService.getAllTasks().pipe(
      finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      })
    ).subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.applyCategoryFilter();
      },
      error: (err) => {
        this.error = err.message || 'Failed to load tasks';
      }
    });
  }

  private applyCategoryFilter(): void {
    const category = this.selectedCategory?.trim();
    if (!category) {
      this.visibleTasks = [...this.tasks];
      return;
    }

    this.visibleTasks = this.tasks.filter(
      task => (task.category || '').toLowerCase() === category.toLowerCase()
    );
  }

  onToggleStatus(task: Task): void {
    this.taskService.toggleTaskStatus(task).subscribe({
      next: (updated) => {
        this.tasks = this.tasks.map(t => (t.id === updated.id ? updated : t));
        this.applyCategoryFilter();
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.message || 'Failed to update task status';
        this.cdr.markForCheck();
      }
    });
  }

  onDelete(id: string): void {
    if (!confirm('Delete this task?')) {
      return;
    }

    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(t => t.id !== id);
        this.applyCategoryFilter();
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.message || 'Failed to delete task';
        this.cdr.markForCheck();
      }
    });
  }
}
