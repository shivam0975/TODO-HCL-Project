import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TaskFormComponent } from '../../components/task-form/task-form';
import { TaskService } from '../../services/task.service';
import { CreateTaskDto } from '../../models/task.model';

@Component({
  selector: 'app-add-task',
  standalone: true,
  imports: [CommonModule, TaskFormComponent],
  template: `
    <div class="add-page" style="animation: fadeIn 0.3s ease">
      <app-task-form
        [loading]="loading"
        [successMsg]="successMsg"
        [errorMsg]="errorMsg"
        (submitForm)="onSubmit($event)"
      />
    </div>
    <style>
      @keyframes fadeIn { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; } }
    </style>
  `
})
export class AddTaskComponent {
  loading = false;
  successMsg = '';
  errorMsg = '';

  constructor(private taskService: TaskService, private router: Router) {}

  onSubmit(dto: CreateTaskDto | any): void {
    this.loading = true;
    this.successMsg = '';
    this.errorMsg = '';

    try {
      // ✅ Sanitize and validate payload
      if (!dto.title || !dto.title.trim()) {
        throw new Error('Title is required');
      }

      const payload: CreateTaskDto = {
        title: dto.title.trim(),
        category: dto.category?.trim() || null,
        description: dto.description?.trim() || null,
        dueDate: dto.dueDate ? new Date(dto.dueDate).toISOString() : null,
        priority: this.convertPriority(dto.priority)
      };

      this.taskService.createTask(payload).subscribe({
        next: () => {
          this.loading = false;
          this.successMsg = 'Task created successfully!';
          setTimeout(() => this.router.navigate(['/tasks']), 1200);
        },
        error: (err) => {
          this.loading = false;
          this.errorMsg = err.message || 'Failed to create task';
          console.error('Create Task Error:', err);
        }
      });
    } catch (err: any) {
      this.loading = false;
      this.errorMsg = err.message || 'Invalid request data';
    }
  }

  private convertPriority(priority: any): number {
    if (typeof priority === 'number') return priority;
    const priorityMap: { [key: string]: number } = {
      'low': 0,
      'medium': 1,
      'high': 2
    };
    return priorityMap[String(priority).toLowerCase()] || 1;
  }
}