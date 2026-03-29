import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Task, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../../models/task.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './task-form.html',
  styleUrls: ['./task-form.css']
})
export class TaskFormComponent implements OnInit {
  @Input() editMode = false;
  @Input() existingTask: Task | null = null;
  @Input() loading = false;
  @Input() successMsg = '';
  @Input() errorMsg = '';
  @Output() submitForm = new EventEmitter<CreateTaskDto | UpdateTaskDto>();

  form!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      title: [this.existingTask?.title ?? '', [Validators.required, Validators.maxLength(200)]],
      description: [this.existingTask?.description ?? '', Validators.maxLength(1000)],
      priority: [this.existingTask?.priority ? this.getPriorityLabel(this.existingTask.priority) : 'Medium'],
      category: [this.existingTask?.category ?? ''],
      status: [this.existingTask?.status ?? 'Pending'],
      dueDate: [this.formatDateForInput(this.existingTask?.dueDate)]
    });
  }

  private formatDateForInput(dateString: string | null | undefined): string {
    if (!dateString) return '';
    // Convert ISO string (2026-03-29T00:00:00.000Z) to YYYY-MM-DD format for date input
    return new Date(dateString).toISOString().split('T')[0];
  }

  private getPriorityLabel(priority: number): string {
    switch (priority) {
      case 2: return 'High';
      case 1: return 'Medium';
      case 0: return 'Low';
      default: return 'Medium';
    }
  }

  onSubmit(): void {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  const v = this.form.value;

  // Map priority string to number (Low=0, Medium=1, High=2)
  const priorityMap: Record<string, number> = { Low: 0, Medium: 1, High: 2 };
  const priorityNum = priorityMap[v.priority] || 1; // default Medium

  // Normalize due date: convert YYYY-MM-DD to full ISO string if provided
  const dueDateIso = v.dueDate ? new Date(v.dueDate).toISOString() : undefined;

  const payload = this.editMode
    ? {
        title: v.title.trim(),
        description: v.description?.trim() || undefined,
        status: v.status as TaskStatus,
        priority: priorityNum,
        category: v.category?.trim() || undefined,
        dueDate: dueDateIso
      } as UpdateTaskDto
    : {
        title: v.title.trim(),
        description: v.description?.trim() || undefined,
        priority: priorityNum,
        category: v.category?.trim() || undefined,
        dueDate: dueDateIso
      } as CreateTaskDto;

  this.submitForm.emit(payload);
}
}
