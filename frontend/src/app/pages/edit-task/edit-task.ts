import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { of, race } from 'rxjs';
import { catchError, finalize, map, timeout } from 'rxjs/operators';
import { TaskFormComponent } from '../../components/task-form/task-form';
import { TaskService } from '../../services/task.service';
import { Task, CreateTaskDto, UpdateTaskDto } from '../../models/task.model';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [CommonModule, TaskFormComponent],
  templateUrl: './edit-task.html',
  styleUrls: ['./edit-task.css']
})
export class EditTaskComponent implements OnInit, OnDestroy {
  task: Task | null = null;
  fetchLoading = false;
  fetchError = '';
  loading = false;
  successMsg = '';
  errorMsg = '';

  private fetchWatchdog: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchLoading = true;
    this.fetchError = '';

    const routeId = this.route.snapshot.paramMap.get('id');
    const stateTask = (history.state?.task as Task | undefined) ?? undefined;

    if (stateTask && routeId && stateTask.id === routeId) {
      this.task = stateTask;
      this.fetchLoading = false;
      return;
    }

    this.fetchWatchdog = setTimeout(() => {
      if (this.fetchLoading) {
        this.setFetchError('Request timed out. Please try again.');
      }
    }, 15000);

    const id = routeId;
    if (!id || id === 'undefined' || id === 'null') {
      this.setFetchError('Invalid task ID');
      this.clearFetchWatchdog();
      return;
    }

    const byId$ = this.taskService.getTaskById(id).pipe(
      map(task => task ?? null),
      catchError(() => of(null))
    );

    const fromList$ = this.taskService.getAllTasks().pipe(
      map(tasks => tasks.find(t => t.id === id) ?? null),
      catchError(() => of(null))
    );

    race(byId$, fromList$).pipe(
      timeout(10000),
      catchError(err => {
        this.fetchError = err?.message || 'Failed to load task';
        return of(null);
      }),
      finalize(() => {
        this.clearFetchWatchdog();
        this.fetchLoading = false;
      })
    ).subscribe(task => {
      if (task) {
        this.task = task;
        return;
      }

      if (!this.fetchError) {
        this.fetchError = 'Task not found';
      }
    });
  }

  ngOnDestroy(): void {
    this.clearFetchWatchdog();
  }

  onSubmit(dto: CreateTaskDto | UpdateTaskDto): void {
    if (!this.task) {
      return;
    }

    this.loading = true;
    this.successMsg = '';
    this.errorMsg = '';

    const submittedStatus = 'status' in dto ? dto.status : this.task.status;

    const payload: UpdateTaskDto = {
      title: (dto.title || '').trim(),
      description: dto.description?.trim() || null,
      status: this.normalizeStatus(submittedStatus),
      priority: this.convertPriority(dto.priority),
      category: dto.category?.trim() || null,
      dueDate: dto.dueDate ? new Date(dto.dueDate).toISOString() : null
    };

    this.taskService.updateTask(this.task.id, payload).subscribe({
      next: () => {
        this.loading = false;
        this.successMsg = 'Task updated successfully!';
        setTimeout(() => this.router.navigate(['/tasks']), 1200);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.message || 'Failed to update task';
      }
    });
  }

  private setFetchError(message: string): void {
    this.fetchError = message;
    this.fetchLoading = false;
  }

  private clearFetchWatchdog(): void {
    if (!this.fetchWatchdog) {
      return;
    }
    clearTimeout(this.fetchWatchdog);
    this.fetchWatchdog = null;
  }

  private normalizeStatus(status: Task['status'] | string): Task['status'] {
    const normalized = String(status || '').toLowerCase();
    return normalized === 'completed' ? 'Completed' : 'Pending';
  }

  private convertPriority(priority: number | string | null | undefined): number {
    if (typeof priority === 'number') {
      return priority === 2 ? 2 : priority === 1 ? 1 : 0;
    }

    const normalized = String(priority || '').toLowerCase();
    if (normalized === 'high') {
      return 2;
    }
    if (normalized === 'medium') {
      return 1;
    }
    return 0;
  }
}